using Microsoft.EntityFrameworkCore;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Domain.Enums;
using Motel.Infrastructure.Data;

namespace Motel.Infrastructure.Services;

public class ReservationsService : IReservationsService
{
    private readonly AppDbContext _context;
    private readonly INotificationsService _notificationsService;

    public ReservationsService(AppDbContext context, INotificationsService notificationsService)
    {
        _context = context;
        _notificationsService = notificationsService;
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
        // Check for overlaps
        var hasOverlap = await HasOverlapAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (hasOverlap)
            throw new InvalidOperationException("الغرفة محجوزة في هذه الفترة");

        // Verify room capacity
        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null)
            throw new InvalidOperationException("الغرفة غير موجودة");

        if (room.Capacity < dto.Guests)
            throw new InvalidOperationException("سعة الغرفة غير كافية");

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = dto.ClientId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            Guests = dto.Guests,
            NightlyRate = dto.NightlyRate,
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

        // Send booking confirmation
        await _notificationsService.SendBookingConfirmationAsync(reservation.Id);

        // Schedule checkout reminder
        var checkoutReminderTime = dto.CheckOutDate.ToDateTime(new TimeOnly(10, 0))
            .AddDays(-1).ToUniversalTime();
        await _notificationsService.ScheduleCheckoutReminderAsync(reservation.Id, checkoutReminderTime);

        return await GetByIdAsync(reservation.Id) ?? throw new InvalidOperationException();
    }

    public async Task<ReservationDto> CheckInAsync(Guid reservationId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
            throw new InvalidOperationException("الحجز غير موجود");

        if (reservation.CheckedIn)
            throw new InvalidOperationException("تم تسجيل الدخول بالفعل");

        reservation.CheckedIn = true;
        reservation.Room.Status = RoomStatus.Occupied;

        await _context.SaveChangesAsync();

        // Send check-in notice
        await _notificationsService.SendCheckInNoticeAsync(reservationId);

        return await GetByIdAsync(reservationId) ?? throw new InvalidOperationException();
    }

    public async Task<ReservationDto> CheckOutAsync(Guid reservationId, decimal? extraCharges = null)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .Include(r => r.Invoice)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
            throw new InvalidOperationException("الحجز غير موجود");

        if (!reservation.CheckedIn)
            throw new InvalidOperationException("لم يتم تسجيل الدخول بعد");

        if (reservation.CheckedOut)
            throw new InvalidOperationException("تم تسجيل الخروج بالفعل");

        reservation.CheckedOut = true;
        reservation.Room.Status = RoomStatus.Available;

        if (extraCharges.HasValue)
            reservation.ExtraCharges = (reservation.ExtraCharges ?? 0) + extraCharges.Value;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(reservationId) ?? throw new InvalidOperationException();
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
