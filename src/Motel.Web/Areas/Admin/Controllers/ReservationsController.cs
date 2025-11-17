using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;

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

        var invoice = await _invoicesService.GetByReservationIdAsync(id);
        ViewBag.Invoice = invoice;

        return View(reservation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(Guid id)
    {
        try
        {
            await _reservationsService.CheckInAsync(id);
            TempData["Success"] = "تم تسجيل الدخول بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(Guid id, decimal? extraCharges)
    {
        try
        {
            await _reservationsService.CheckOutAsync(id, extraCharges);
            TempData["Success"] = "تم تسجيل الخروج بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateDropdowns()
    {
        var clients = await _clientsService.GetAllAsync();
        var rooms = await _roomsService.GetAllAsync();

        ViewBag.Clients = new SelectList(clients, "Id", "FullName");
        ViewBag.Rooms = new SelectList(rooms, "Id", "Number");
    }
}
