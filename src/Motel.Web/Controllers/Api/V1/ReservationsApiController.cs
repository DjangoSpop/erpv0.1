using Microsoft.AspNetCore.Mvc;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;

namespace Motel.Web.Controllers.Api.V1;

[ApiController]
[Route("api/v1/reservations")]
public class ReservationsApiController : ControllerBase
{
    private readonly IReservationsService _reservationsService;

    public ReservationsApiController(IReservationsService reservationsService)
    {
        _reservationsService = reservationsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
    {
        var reservations = await _reservationsService.GetAllAsync();
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetById(Guid id)
    {
        var reservation = await _reservationsService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound(new { message = "الحجز غير موجود" });

        return Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto dto)
    {
        try
        {
            var reservation = await _reservationsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/check-in")]
    public async Task<ActionResult<ReservationDto>> CheckIn(Guid id)
    {
        try
        {
            var reservation = await _reservationsService.CheckInAsync(id);
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/check-out")]
    public async Task<ActionResult<ReservationDto>> CheckOut(Guid id, [FromBody] CheckOutDto? dto = null)
    {
        try
        {
            var reservation = await _reservationsService.CheckOutAsync(id, dto?.ExtraCharges);
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
