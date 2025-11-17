using Microsoft.EntityFrameworkCore;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Enums;
using Motel.Infrastructure.Data;
using System.Globalization;

namespace Motel.Infrastructure.Services;

public class ReportsService : IReportsService
{
    private readonly AppDbContext _context;

    public ReportsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var todayCheckIns = await _context.Reservations
            .CountAsync(r => r.CheckInDate == today);

        var todayCheckOuts = await _context.Reservations
            .CountAsync(r => r.CheckOutDate == today);

        var availableRooms = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Available);

        var occupiedRooms = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Occupied);

        var reservedRooms = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Reserved);

        var totalRooms = await _context.Rooms
            .CountAsync(r => r.Status != RoomStatus.OutOfService);

        var occupancyRate = totalRooms > 0
            ? (decimal)(occupiedRooms + reservedRooms) / totalRooms * 100
            : 0;

        var todayRevenue = await _context.Invoices
            .Where(i => i.PaidAtUtc.HasValue &&
                       i.PaidAtUtc.Value.Date == DateTime.UtcNow.Date &&
                       i.Status == InvoiceStatus.Paid)
            .SumAsync(i => (decimal?)i.Total) ?? 0;

        var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var monthRevenue = await _context.Invoices
            .Where(i => i.PaidAtUtc.HasValue &&
                       i.PaidAtUtc.Value >= monthStart &&
                       i.Status == InvoiceStatus.Paid)
            .SumAsync(i => (decimal?)i.Total) ?? 0;

        return new DashboardDto
        {
            TodayCheckIns = todayCheckIns,
            TodayCheckOuts = todayCheckOuts,
            AvailableRooms = availableRooms,
            OccupiedRooms = occupiedRooms,
            ReservedRooms = reservedRooms,
            OccupancyRate = occupancyRate,
            TodayRevenue = todayRevenue,
            MonthRevenue = monthRevenue
        };
    }

    public async Task<IEnumerable<DailyOccupancyDto>> GetDailyOccupancyAsync(DateOnly date)
    {
        var totalRooms = await _context.Rooms.CountAsync();
        var totalActiveRooms = await _context.Rooms
            .CountAsync(r => r.Status != RoomStatus.OutOfService);

        var available = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Available);

        var occupied = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Occupied);

        var reserved = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.Reserved);

        var outOfService = await _context.Rooms
            .CountAsync(r => r.Status == RoomStatus.OutOfService);

        var occupancyRate = totalActiveRooms > 0
            ? (decimal)(occupied + reserved) / totalActiveRooms * 100
            : 0;

        return new List<DailyOccupancyDto>
        {
            new()
            {
                Date = date,
                Available = available,
                Occupied = occupied,
                Reserved = reserved,
                OutOfService = outOfService,
                OccupancyRate = occupancyRate
            }
        };
    }

    public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year)
    {
        var invoices = await _context.Invoices
            .Where(i => i.PaidAtUtc.HasValue &&
                       i.PaidAtUtc.Value.Year == year &&
                       i.Status == InvoiceStatus.Paid)
            .ToListAsync();

        var culture = new CultureInfo("ar-EG");
        var monthlyData = invoices
            .GroupBy(i => i.PaidAtUtc!.Value.Month)
            .Select(g => new MonthlyRevenueDto
            {
                Year = year,
                Month = g.Key,
                MonthName = culture.DateTimeFormat.GetMonthName(g.Key),
                InvoiceCount = g.Count(),
                TotalRevenue = g.Sum(i => i.Total)
            })
            .OrderBy(m => m.Month)
            .ToList();

        return monthlyData;
    }
}
