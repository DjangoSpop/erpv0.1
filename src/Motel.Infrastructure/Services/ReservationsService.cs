using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Domain.Enums;
using Motel.Domain.Exceptions;
using Motel.Infrastructure.Data;

namespace Motel.Infrastructure.Services;

public class ReservationsService : IReservationsService
{
    private readonly AppDbContext _context;
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<ReservationsService> _logger;

    public ReservationsService(
        AppDbContext context,
        INotificationsService notificationsService,
        ILogger<ReservationsService> logger)
    {
        _context = context;
        _notificationsService = notificationsService;
        _logger = logger;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync()
    {
        return await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Room)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);

        return reservation == null ? null : MapToDto(reservation);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        _logger.LogInformation("Creating reservation for Client {ClientId} in Room {RoomId} from {CheckIn} to {CheckOut}",
            dto.ClientId, dto.RoomId, dto.CheckInDate, dto.CheckOutDate);

        // Validate dates
        if (dto.CheckOutDate <= dto.CheckInDate)
        {
            _logger.LogWarning("Invalid dates: CheckOut {CheckOut} must be after CheckIn {CheckIn}",
                dto.CheckOutDate, dto.CheckInDate);
            throw new ValidationException("Check-out date must be after check-in date");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (dto.CheckInDate < today)
        {
            _logger.LogWarning("Invalid check-in date: {CheckIn} is in the past", dto.CheckInDate);
            throw new ValidationException("Check-in date cannot be in the past");
        }

        // Verify client exists
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);
        if (!clientExists)
        {
            _logger.LogWarning("Client {ClientId} not found", dto.ClientId);
            throw new EntityNotFoundException("Client", dto.ClientId);
        }

        // Verify room exists and get capacity
        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} not found", dto.RoomId);
            throw new EntityNotFoundException("Room", dto.RoomId);
        }

        if (room.Status == RoomStatus.OutOfService)
        {
            _logger.LogWarning("Room {RoomId} is out of service", dto.RoomId);
            throw new MotelBusinessException($"Room {room.Number} is currently out of service", "ROOM_OUT_OF_SERVICE");
        }

        if (room.Capacity < dto.Guests)
        {
            _logger.LogWarning("Room {RoomId} capacity {Capacity} insufficient for {Guests} guests",
                dto.RoomId, room.Capacity, dto.Guests);
            throw new ValidationException($"Room {room.Number} can accommodate maximum {room.Capacity} guests");
        }

        // Check for overlapping reservations
        var hasOverlap = await HasOverlapAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (hasOverlap)
        {
            _logger.LogWarning("Room {RoomId} has overlapping reservation for {CheckIn} to {CheckOut}",
                dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
            throw new RoomNotAvailableException(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        }

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = dto.ClientId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            Guests = dto.Guests,
            NightlyRate = dto.NightlyRate > 0 ? dto.NightlyRate : room.BaseNightlyRate,
            DiscountAmount = dto.DiscountAmount,
            ExtraCharges = dto.ExtraCharges,
            Notes = dto.Notes,
            CheckedIn = false,
            CheckedOut = false
        };

        _context.Reservations.Add(reservation);

        // Update room status to Reserved
        room.Status = RoomStatus.Reserved;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Reservation {ReservationId} created successfully", reservation.Id);

        try
        {
            // Send booking confirmation
            await _notificationsService.SendBookingConfirmationAsync(reservation.Id);

            // Schedule checkout reminder
            var checkoutReminderTime = dto.CheckOutDate.ToDateTime(new TimeOnly(10, 0))
                .AddDays(-1).ToUniversalTime();
            await _notificationsService.ScheduleCheckoutReminderAsync(reservation.Id, checkoutReminderTime);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send notification for reservation {ReservationId}", reservation.Id);
            // Don't fail the reservation creation if notification fails
        }

        return await GetByIdAsync(reservation.Id) ?? throw new EntityNotFoundException("Reservation", reservation.Id);
    }

    public async Task<ReservationDto> CheckInAsync(Guid reservationId)
    {
        _logger.LogInformation("Processing check-in for reservation {ReservationId}", reservationId);

        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            _logger.LogWarning("Reservation {ReservationId} not found", reservationId);
            throw new EntityNotFoundException("Reservation", reservationId);
        }

        if (reservation.CheckedIn)
        {
            _logger.LogWarning("Reservation {ReservationId} already checked in", reservationId);
            throw new InvalidReservationStateException(reservationId, "Checked In", "Confirmed");
        }

        if (reservation.CheckedOut)
        {
            _logger.LogWarning("Cannot check in: Reservation {ReservationId} already checked out", reservationId);
            throw new InvalidReservationStateException(reservationId, "Checked Out", "Confirmed");
        }

        // Check if check-in date is valid (allow check-in 1 day before)
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (reservation.CheckInDate > today.AddDays(1))
        {
            _logger.LogWarning("Check-in date {CheckInDate} is too far in the future", reservation.CheckInDate);
            throw new ValidationException($"Check-in is scheduled for {reservation.CheckInDate:yyyy-MM-dd}. Too early to check in.");
        }

        reservation.CheckedIn = true;
        reservation.Room.Status = RoomStatus.Occupied;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Check-in completed for reservation {ReservationId}", reservationId);

        try
        {
            await _notificationsService.SendCheckInNoticeAsync(reservationId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send check-in notification for reservation {ReservationId}", reservationId);
        }

        return await GetByIdAsync(reservationId) ?? throw new EntityNotFoundException("Reservation", reservationId);
    }

    public async Task<ReservationDto> CheckOutAsync(Guid reservationId, decimal? extraCharges = null)
    {
        _logger.LogInformation("Processing check-out for reservation {ReservationId} with extra charges: {ExtraCharges}",
            reservationId, extraCharges);

        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .Include(r => r.Invoice)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            _logger.LogWarning("Reservation {ReservationId} not found", reservationId);
            throw new EntityNotFoundException("Reservation", reservationId);
        }

        if (!reservation.CheckedIn)
        {
            _logger.LogWarning("Cannot check out: Reservation {ReservationId} not checked in yet", reservationId);
            throw new InvalidReservationStateException(reservationId, "Not Checked In", "Checked In");
        }

        if (reservation.CheckedOut)
        {
            _logger.LogWarning("Reservation {ReservationId} already checked out", reservationId);
            throw new InvalidReservationStateException(reservationId, "Checked Out", "Checked In");
        }

        reservation.CheckedOut = true;
        reservation.Room.Status = RoomStatus.Available;

        if (extraCharges.HasValue && extraCharges.Value < 0)
        {
            _logger.LogWarning("Invalid extra charges: {ExtraCharges}", extraCharges.Value);
            throw new ValidationException("Extra charges cannot be negative");
        }

        if (extraCharges.HasValue)
        {
            reservation.ExtraCharges = (reservation.ExtraCharges ?? 0) + extraCharges.Value;
            _logger.LogInformation("Added extra charges {Amount} to reservation {ReservationId}",
                extraCharges.Value, reservationId);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Check-out completed for reservation {ReservationId}", reservationId);

        return await GetByIdAsync(reservationId) ?? throw new EntityNotFoundException("Reservation", reservationId);
    }

    public async Task<IEnumerable<ReservationDto>> GetByDateRangeAsync(DateOnly from, DateOnly to)
    {
        return await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Room)
            .Where(r => r.CheckInDate <= to && r.CheckOutDate >= from)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, Guid? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.RoomId == roomId && !r.CheckedOut);

        if (excludeReservationId.HasValue)
            query = query.Where(r => r.Id != excludeReservationId.Value);

        return await query.AnyAsync(r =>
            !(checkOut <= r.CheckInDate || checkIn >= r.CheckOutDate));
    }

    private static ReservationDto MapToDto(Reservation r)
    {
        var nights = Math.Max(1, (r.CheckOutDate.DayNumber - r.CheckInDate.DayNumber));
        var subtotal = (r.NightlyRate * nights) + (r.ExtraCharges ?? 0) - (r.DiscountAmount ?? 0);

        return new ReservationDto
        {
            Id = r.Id,
            ClientId = r.ClientId,
            ClientName = r.Client.FullName,
            RoomId = r.RoomId,
            RoomNumber = r.Room.Number,
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            Guests = r.Guests,
            CheckedIn = r.CheckedIn,
            CheckedOut = r.CheckedOut,
            NightlyRate = r.NightlyRate,
            DiscountAmount = r.DiscountAmount,
            ExtraCharges = r.ExtraCharges,
            Notes = r.Notes,
            Nights = nights,
            TotalAmount = subtotal
        };
    }
}
