using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Enums;
using Motel.Web.Extensions;
using Motel.Web.ViewModels.Reservations;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ReservationsController : Controller
{
    private readonly IReservationsService _reservationsService;
    private readonly IRoomsService _roomsService;
    private readonly IClientsService _clientsService;
    private readonly IInvoicesService _invoicesService;

    public ReservationsController(
        IReservationsService reservationsService,
        IRoomsService roomsService,
        IClientsService clientsService,
        IInvoicesService invoicesService)
    {
        _reservationsService = reservationsService;
        _roomsService = roomsService;
        _clientsService = clientsService;
        _invoicesService = invoicesService;
    }

    public async Task<IActionResult> Index(string? dateFrom, string? dateTo)
    {
        IEnumerable<ReservationDto> reservations;

        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
        {
            var from = DateOnly.Parse(dateFrom);
            var to = DateOnly.Parse(dateTo);
            reservations = await _reservationsService.GetByDateRangeAsync(from, to);
        }
        else
        {
            reservations = await _reservationsService.GetAllAsync();
        }

        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;
        return View(reservations);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(dto);
        }

        try
        {
            await _reservationsService.CreateAsync(dto);
            TempData["Success"] = "تم إنشاء الحجز بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateDropdowns();
            return View(dto);
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var reservation = await _reservationsService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        var viewModel = reservation.ToDetailViewModel();

        // Check if invoice exists
        var invoice = await _invoicesService.GetByReservationIdAsync(id);
        if (invoice != null)
        {
            viewModel.InvoiceId = invoice.Id;
            viewModel.InvoiceSerial = invoice.Serial;
        }

        return View(viewModel);
    }

    // GET: CheckIn form
    public async Task<IActionResult> CheckIn(Guid id)
    {
        var reservation = await _reservationsService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        if (reservation.Status != ReservationStatus.Confirmed)
        {
            TempData["Error"] = "لا يمكن تسجيل الدخول لهذا الحجز في حالته الحالية";
            return RedirectToAction(nameof(Details), new { id });
        }

        var viewModel = reservation.ToCheckInViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(CheckInViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // For now, use the simple CheckInAsync from service
            // In future, we can enhance the service to accept the full model
            await _reservationsService.CheckInAsync(model.ReservationId);

            // TODO: Store additional check-in details (documents, special requests, etc.)

            TempData["Success"] = "تم تسجيل الدخول بنجاح";
            return RedirectToAction(nameof(Details), new { id = model.ReservationId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    // GET: CheckOut form
    public async Task<IActionResult> CheckOut(Guid id)
    {
        var reservation = await _reservationsService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        if (reservation.Status != ReservationStatus.CheckedIn)
        {
            TempData["Error"] = "لا يمكن تسجيل الخروج لهذا الحجز في حالته الحالية";
            return RedirectToAction(nameof(Details), new { id });
        }

        var viewModel = reservation.ToCheckOutViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(CheckOutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Calculate total extra charges
            var totalExtraCharges = model.TotalExtraCharges;

            await _reservationsService.CheckOutAsync(model.ReservationId, totalExtraCharges);

            // TODO: Store detailed checkout information

            TempData["Success"] = "تم تسجيل الخروج بنجاح";
            return RedirectToAction(nameof(Details), new { id = model.ReservationId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    private async Task PopulateDropdowns()
    {
        var clients = await _clientsService.GetAllAsync();
        var rooms = await _roomsService.GetAllAsync();

        ViewBag.Clients = new SelectList(clients, "Id", "FullName");
        ViewBag.Rooms = new SelectList(rooms, "Id", "Number");
    }
}
