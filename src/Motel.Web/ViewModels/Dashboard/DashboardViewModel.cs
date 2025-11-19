namespace Motel.Web.ViewModels.Dashboard;

/// <summary>
/// Comprehensive dashboard with KPIs and recent activity
/// </summary>
public class DashboardViewModel
{
    // KPIs - Today
    public int TodayCheckIns { get; set; }
    public int TodayCheckOuts { get; set; }
    public int PendingCheckIns { get; set; }
    public int PendingCheckOuts { get; set; }

    // Room Statistics
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int ReservedRooms { get; set; }
    public int OutOfServiceRooms { get; set; }
    public decimal OccupancyRate { get; set; }
    public string OccupancyRateFormatted { get; set; } = default!;

    // Revenue - Today
    public decimal TodayRevenue { get; set; }
    public string TodayRevenueFormatted { get; set; } = default!;
    public int TodayInvoices { get; set; }

    // Revenue - Month
    public decimal MonthRevenue { get; set; }
    public string MonthRevenueFormatted { get; set; } = default!;
    public int MonthInvoices { get; set; }

    // Revenue - Year
    public decimal YearRevenue { get; set; }
    public string YearRevenueFormatted { get; set; } = default!;

    // Pending Payments
    public decimal PendingPayments { get; set; }
    public string PendingPaymentsFormatted { get; set; } = default!;
    public int PendingInvoicesCount { get; set; }

    // Recent Activity
    public List<RecentActivity> RecentActivities { get; set; } = new();

    // Upcoming Events
    public List<UpcomingEvent> UpcomingEvents { get; set; } = new();

    // Charts Data
    public List<OccupancyTrendData> OccupancyTrend { get; set; } = new();
    public List<RevenueTrendData> RevenueTrend { get; set; } = new();

    // Alerts
    public List<DashboardAlert> Alerts { get; set; } = new();

    // Quick Stats
    public int TotalClients { get; set; }
    public int NewClientsThisMonth { get; set; }
    public int TotalReservationsThisMonth { get; set; }
    public decimal AverageStayDuration { get; set; }
    public decimal AverageRevenuePerBooking { get; set; }
}

public class RecentActivity
{
    public string Icon { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public string TimeAgo { get; set; } = default!;
    public string? Link { get; set; }
}

public class UpcomingEvent
{
    public string Type { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime ScheduledTime { get; set; }
    public string TimeUntil { get; set; } = default!;
    public string Priority { get; set; } = default!;
}

public class OccupancyTrendData
{
    public string Date { get; set; } = default!;
    public decimal OccupancyRate { get; set; }
}

public class RevenueTrendData
{
    public string Date { get; set; } = default!;
    public decimal Revenue { get; set; }
}

public class DashboardAlert
{
    public string Type { get; set; } = default!; // info, warning, danger, success
    public string Icon { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? ActionLink { get; set; }
    public string? ActionText { get; set; }
}
