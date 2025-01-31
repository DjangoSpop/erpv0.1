namespace erpv0._1.Models.ViewModels
{
    public class HomeViewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
        public InventoryMetricsViewModel InventoryMetrics { get; set; } = new();
        public List<WarehouseUtilizationViewModel> WarehouseUtilization { get; set; } = new();
    }

    public class DashboardStats
    {
        public int TotalProducts { get; set; }
        public int ActiveWarehouses { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int PendingTransfers { get; set; }
        public int LowStockItems { get; set; }
    }

    public class RecentActivityViewModel
    {
        public DateTime Date { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class InventoryMetricsViewModel
    {
        public int TotalStockEntries { get; set; }
        public int TotalMovements { get; set; }
        public decimal AverageStockValue { get; set; }
        public int StockOutItems { get; set; }
    }

    public class WarehouseUtilizationViewModel
    {
        public string WarehouseName { get; set; } = string.Empty;
        public int TotalCapacity { get; set; }
        public int CurrentStock { get; set; }
        public decimal UtilizationPercentage => TotalCapacity > 0 ?
            CurrentStock * 100.0m / TotalCapacity : 0;
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}