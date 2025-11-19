using Microsoft.AspNetCore.Mvc;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Enums;
using Motel.Web.Extensions;
using Motel.Web.ViewModels.Rooms;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RoomsController : Controller
{
    private readonly IRoomsService _roomsService;

    public RoomsController(IRoomsService roomsService)
    {
        _roomsService = roomsService;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        RoomType? filterByType,
        RoomStatus? filterByStatus,
        int? minCapacity,
        decimal? maxPrice,
        DateOnly? availabilityFromDate,
        DateOnly? availabilityToDate,
        int? requiredGuests)
    {
        IEnumerable<RoomDto> rooms;

        // Check if we need to filter by availability dates
        if (availabilityFromDate.HasValue && availabilityToDate.HasValue)
        {
            var guests = requiredGuests ?? 1;
            rooms = await _roomsService.GetAvailableRoomsAsync(
                availabilityFromDate.Value,
                availabilityToDate.Value,
                guests);
        }
        else
        {
            rooms = await _roomsService.GetAllAsync();
        }

        // Apply additional filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            rooms = rooms.Where(r => r.Number.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (filterByType.HasValue)
        {
            rooms = rooms.Where(r => r.Type == filterByType.Value);
        }

        if (filterByStatus.HasValue)
        {
            rooms = rooms.Where(r => r.Status == filterByStatus.Value);
        }

        if (minCapacity.HasValue)
        {
            rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        }

        if (maxPrice.HasValue)
        {
            rooms = rooms.Where(r => r.BaseNightlyRate <= maxPrice.Value);
        }

        var viewModel = rooms.ToListViewModel(
            searchTerm,
            filterByType,
            filterByStatus,
            minCapacity,
            maxPrice,
            availabilityFromDate,
            availabilityToDate,
            requiredGuests);

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _roomsService.CreateAsync(dto);
            TempData["Success"] = "تم إضافة الغرفة بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var room = await _roomsService.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        var dto = new UpdateRoomDto
        {
            Number = room.Number,
            Type = room.Type,
            BaseNightlyRate = room.BaseNightlyRate,
            Status = room.Status,
            Capacity = room.Capacity,
            Notes = room.Notes
        };

        ViewBag.RoomId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateRoomDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.RoomId = id;
            return View(dto);
        }

        try
        {
            await _roomsService.UpdateAsync(id, dto);
            TempData["Success"] = "تم تحديث الغرفة بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.RoomId = id;
            return View(dto);
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var room = await _roomsService.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        return View(room);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _roomsService.DeleteAsync(id);
            TempData["Success"] = "تم حذف الغرفة بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
