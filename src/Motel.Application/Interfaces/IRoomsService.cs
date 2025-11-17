using Motel.Application.DTOs;
using Motel.Domain.Enums;

namespace Motel.Application.Interfaces;

public interface IRoomsService
{
    Task<IEnumerable<RoomDto>> GetAllAsync();
    Task<RoomDto?> GetByIdAsync(Guid id);
    Task<RoomDto> CreateAsync(CreateRoomDto dto);
    Task<RoomDto> UpdateAsync(Guid id, UpdateRoomDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateOnly checkIn, DateOnly checkOut, int guests);
    Task<bool> SetRoomStatusAsync(Guid id, RoomStatus status);
}
