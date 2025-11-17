using Motel.Application.DTOs;

namespace Motel.Application.Interfaces;

public interface IReportsService
{
    Task<DashboardDto> GetDashboardDataAsync();
    Task<IEnumerable<DailyOccupancyDto>> GetDailyOccupancyAsync(DateOnly date);
    Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year);
}

public class DailyOccupancyDto
{
    public DateOnly Date { get; set; }
    public int Available { get; set; }
    public int Occupied { get; set; }
    public int Reserved { get; set; }
    public int OutOfService { get; set; }
    public decimal OccupancyRate { get; set; }
}

public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = default!;
    public int InvoiceCount { get; set; }
    public decimal TotalRevenue { get; set; }
}
