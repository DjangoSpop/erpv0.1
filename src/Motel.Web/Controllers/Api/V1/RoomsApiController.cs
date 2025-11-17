using Microsoft.AspNetCore.Mvc;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Enums;

namespace Motel.Web.Controllers.Api.V1;

[ApiController]
[Route("api/v1/rooms")]
public class RoomsApiController : ControllerBase
{
    private readonly IRoomsService _roomsService;

    public RoomsApiController(IRoomsService roomsService)
    {
        _roomsService = roomsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll(
        [FromQuery] RoomStatus? status = null,
        [FromQuery] string? dateFrom = null,
        [FromQuery] string? dateTo = null,
        [FromQuery] int? guests = null)
    {
        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && guests.HasValue)
        {
            var checkIn = DateOnly.Parse(dateFrom);
            var checkOut = DateOnly.Parse(dateTo);
            var availableRooms = await _roomsService.GetAvailableRoomsAsync(checkIn, checkOut, guests.Value);
            return Ok(availableRooms);
        }

        var rooms = await _roomsService.GetAllAsync();

        if (status.HasValue)
            rooms = rooms.Where(r => r.Status == status.Value);

        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id)
    {
        var room = await _roomsService.GetByIdAsync(id);
        if (room == null)
            return NotFound(new { message = "الغرفة غير موجودة" });

        return Ok(room);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
    {
        try
        {
            var room = await _roomsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> Update(Guid id, [FromBody] UpdateRoomDto dto)
    {
        try
        {
            var room = await _roomsService.UpdateAsync(id, dto);
            return Ok(room);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _roomsService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
