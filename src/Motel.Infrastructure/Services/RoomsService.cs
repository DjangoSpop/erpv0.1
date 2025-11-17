using Microsoft.EntityFrameworkCore;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Domain.Enums;
using Motel.Infrastructure.Data;

namespace Motel.Infrastructure.Services;

public class RoomsService : IRoomsService
{
    private readonly AppDbContext _context;

    public RoomsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync()
    {
        return await _context.Rooms
            .Select(r => new RoomDto
            {
                Id = r.Id,
                Number = r.Number,
                Type = r.Type,
                BaseNightlyRate = r.BaseNightlyRate,
                Status = r.Status,
                Capacity = r.Capacity,
                Notes = r.Notes
            })
            .ToListAsync();
    }

    public async Task<RoomDto?> GetByIdAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return null;

        return new RoomDto
        {
            Id = room.Id,
            Number = room.Number,
            Type = room.Type,
            BaseNightlyRate = room.BaseNightlyRate,
            Status = room.Status,
            Capacity = room.Capacity,
            Notes = room.Notes
        };
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Number = dto.Number,
            Type = dto.Type,
            BaseNightlyRate = dto.BaseNightlyRate,
            Capacity = dto.Capacity,
            Notes = dto.Notes,
            Status = RoomStatus.Available
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomDto
        {
            Id = room.Id,
            Number = room.Number,
            Type = room.Type,
            BaseNightlyRate = room.BaseNightlyRate,
            Status = room.Status,
            Capacity = room.Capacity,
            Notes = room.Notes
        };
    }

    public async Task<RoomDto> UpdateAsync(Guid id, UpdateRoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new InvalidOperationException("الغرفة غير موجودة");

        room.Number = dto.Number;
        room.Type = dto.Type;
        room.BaseNightlyRate = dto.BaseNightlyRate;
        room.Status = dto.Status;
        room.Capacity = dto.Capacity;
        room.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return new RoomDto
        {
            Id = room.Id,
            Number = room.Number,
            Type = room.Type,
            BaseNightlyRate = room.BaseNightlyRate,
            Status = room.Status,
            Capacity = room.Capacity,
            Notes = room.Notes
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new InvalidOperationException("الغرفة غير موجودة");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateOnly checkIn, DateOnly checkOut, int guests)
    {
        // Get all rooms that match capacity
        var rooms = await _context.Rooms
            .Where(r => r.Capacity >= guests && r.Status != RoomStatus.OutOfService)
            .ToListAsync();

        // Check for overlapping reservations
        var availableRooms = new List<RoomDto>();
        foreach (var room in rooms)
        {
            var hasOverlap = await _context.Reservations
                .AnyAsync(r => r.RoomId == room.Id &&
                              !r.CheckedOut &&
                              !(checkOut <= r.CheckInDate || checkIn >= r.CheckOutDate));

            if (!hasOverlap)
            {
                availableRooms.Add(new RoomDto
                {
                    Id = room.Id,
                    Number = room.Number,
                    Type = room.Type,
                    BaseNightlyRate = room.BaseNightlyRate,
                    Status = room.Status,
                    Capacity = room.Capacity,
                    Notes = room.Notes
                });
            }
        }

        return availableRooms;
    }

    public async Task<bool> SetRoomStatusAsync(Guid id, RoomStatus status)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;

        room.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
