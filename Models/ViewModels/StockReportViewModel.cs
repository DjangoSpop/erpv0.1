namespace erpv0._1.Models.ViewModels
{
    public class StockReportViewModel
    {
        public StockStatistics Statistics { get; set; } = new();
        public List<StockMovementSummary> RecentMovements { get; set; } = new();
        public List<StockProductDetail> Products { get; set; } = new();
        public List<StockTrendData> TrendData { get; set; } = new();
        public List<CategoryDistribution> CategoryDistribution { get; set; } = new();
    }

    public class StockStatistics
    {
        // General Statistics
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageStockValue { get; set; }

        // Stock Levels
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int OptimalStockItems { get; set; }
        public int OverStockItems { get; set; }

        // Movement Statistics
        public int TotalMovements { get; set; }
        public int IncomingMovements { get; set; }
        public int OutgoingMovements { get; set; }
        public int PendingTransfers { get; set; }

        // Time-based Statistics
        public DateTime LastUpdated { get; set; }
        public decimal DailyAverageMovement { get; set; }
        public int StockTurnoverRate { get; set; }
    }

    public class StockMovementSummary
    {
        public int MovementId { get; set; }
        public DateTime Date { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitValue { get; set; }
        public decimal TotalValue { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string InitiatedBy { get; set; } = string.Empty;
    }

    public class StockProductDetail
    {
        // Basic Information
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;

        // Stock Information
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderPoint { get; set; }
        public string Location { get; set; } = string.Empty;

        // Value Information
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public decimal ProfitMargin { get; set; }

        // Movement Information
        public DateTime? LastReceived { get; set; }
        public DateTime? LastIssued { get; set; }
        public int MonthlyAverageDemand { get; set; }
        public int PendingOrders { get; set; }

        // Status Calculations
        public string StockStatus => CalculateStockStatus();
        public decimal StockLevel => CalculateStockLevel();

        private string CalculateStockStatus()
        {
            if (CurrentStock <= 0) return "نفذ المخزون";
            if (CurrentStock <= MinimumStock) return "منخفض";
            if (CurrentStock >= MaximumStock) return "فائض";
            return "متوفر";
        }

        private decimal CalculateStockLevel()
        {
            if (MaximumStock == 0) return 0;
            return (decimal)CurrentStock / MaximumStock * 100;
        }
    }

    public class StockTrendData
    {
        public DateTime Date { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalValue { get; set; }
        public int IncomingQuantity { get; set; }
        public int OutgoingQuantity { get; set; }
        public decimal AverageUnitPrice { get; set; }
    }

    public class CategoryDistribution
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public List<StockLevelDistribution> StockLevels { get; set; } = new();
    }

    public class StockLevelDistribution
    {
        public string Level { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    // Enums for consistent status handling
    public enum StockLevel
    {
        OutOfStock,
        Low,
        Optimal,
        Excess
    }

    public enum MovementType
    {
        Purchase,
        Sale,
        Transfer,
        Adjustment,
        Return,
        Damage
    }
}

