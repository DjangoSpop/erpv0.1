using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Domain.Enums;
using Motel.Domain.Exceptions;
using Motel.Infrastructure.Data;

namespace Motel.Infrastructure.Services;

public class RoomsService : IRoomsService
{
    private readonly AppDbContext _context;
    private readonly ILogger<RoomsService> _logger;

    public RoomsService(AppDbContext context, ILogger<RoomsService> logger)
    {
        _context = context;
        _logger = logger;
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
        _logger.LogInformation("Creating new room with number: {RoomNumber}", dto.Number);

        // Check for duplicate room number
        var existingRoom = await _context.Rooms
            .IgnoreQueryFilters() // Include soft-deleted records
            .FirstOrDefaultAsync(r => r.Number == dto.Number);

        if (existingRoom != null)
        {
            if (existingRoom.IsDeleted)
            {
                _logger.LogWarning("Room {RoomNumber} exists but is soft-deleted. Restoring it.", dto.Number);
                // Restore soft-deleted room
                existingRoom.IsDeleted = false;
                existingRoom.Type = dto.Type;
                existingRoom.BaseNightlyRate = dto.BaseNightlyRate;
                existingRoom.Capacity = dto.Capacity;
                existingRoom.Notes = dto.Notes;
                existingRoom.Status = RoomStatus.Available;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Room {RoomNumber} restored successfully with ID: {RoomId}", existingRoom.Number, existingRoom.Id);
                return MapToDto(existingRoom);
            }
            else
            {
                _logger.LogWarning("Duplicate room number detected: {RoomNumber}", dto.Number);
                throw new DuplicateEntityException("Room", "Number", dto.Number);
            }
        }

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

        _logger.LogInformation("Room {RoomNumber} created successfully with ID: {RoomId}", room.Number, room.Id);
        return MapToDto(room);
    }

    private static RoomDto MapToDto(Room room)
    {
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
        _logger.LogInformation("Updating room {RoomId}", id);

        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} not found", id);
            throw new EntityNotFoundException("Room", id);
        }

        // Check for duplicate room number (excluding current room)
        if (room.Number != dto.Number)
        {
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Number == dto.Number && r.Id != id);

            if (existingRoom != null)
            {
                _logger.LogWarning("Duplicate room number detected: {RoomNumber}", dto.Number);
                throw new DuplicateEntityException("Room", "Number", dto.Number);
            }
        }

        room.Number = dto.Number;
        room.Type = dto.Type;
        room.BaseNightlyRate = dto.BaseNightlyRate;
        room.Status = dto.Status;
        room.Capacity = dto.Capacity;
        room.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Room {RoomId} updated successfully", id);
        return MapToDto(room);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting room {RoomId}", id);

        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} not found", id);
            throw new EntityNotFoundException("Room", id);
        }

        // Check for active reservations
        var hasActiveReservations = await _context.Reservations
            .AnyAsync(r => r.RoomId == id && !r.CheckedOut);

        if (hasActiveReservations)
        {
            _logger.LogWarning("Cannot delete room {RoomId} - has active reservations", id);
            throw new EntityHasDependenciesException("Room", id, "active reservations");
        }

        // Soft delete (handled automatically by SaveChangesAsync override)
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room {RoomId} soft-deleted successfully", id);
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
