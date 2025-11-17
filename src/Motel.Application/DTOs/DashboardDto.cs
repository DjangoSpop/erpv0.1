namespace Motel.Application.DTOs;

public class DashboardDto
{
    public int TodayCheckIns { get; set; }
    public int TodayCheckOuts { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int ReservedRooms { get; set; }
    public decimal OccupancyRate { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
}
