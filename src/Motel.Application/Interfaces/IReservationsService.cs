using Motel.Application.DTOs;

namespace Motel.Application.Interfaces;

public interface IReservationsService
{
    Task<IEnumerable<ReservationDto>> GetAllAsync();
    Task<ReservationDto?> GetByIdAsync(Guid id);
    Task<ReservationDto> CreateAsync(CreateReservationDto dto);
    Task<ReservationDto> CheckInAsync(Guid reservationId);
    Task<ReservationDto> CheckOutAsync(Guid reservationId, decimal? extraCharges = null);
    Task<IEnumerable<ReservationDto>> GetByDateRangeAsync(DateOnly from, DateOnly to);
    Task<bool> HasOverlapAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, Guid? excludeReservationId = null);
}
