namespace erpv0._1.Models.ViewModels
{
    public class ReportFilterViewModel
    {
        public string SearchTerm { get; set; }
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public int? CategoryId { get; set; }
        public int? StaffId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    }

    public class ReportSummaryViewModel
    {
        public decimal TotalInventoryValue { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public decimal AverageDailyTransactions { get; set; }
        public int PendingTransfers { get; set; }
    }

    public class StockEntryReportViewModel
    {
        public int EntryId { get; set; }
        public string BatchNumber { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string SupplierInvoice { get; set; }
        public decimal TotalValue => Quantity * CostPrice;
        public string Status => Quantity switch
        {
            <= 0 => "Out of Stock",
            < 10 => "Low Stock",
            _ => "In Stock"
        };
    }

    public class ProductVariantReportViewModel
    {
        public int VariantId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Style { get; set; }
        public decimal PriceAdjustment { get; set; }
        public int StockLevel { get; set; }
    }

    public class WarehouseStockReportViewModel
    {
        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockItems { get; set; }
        public double CapacityUtilization { get; set; }
    }

    public class ReportsIndexViewModel
    {
        public ReportFilterViewModel Filters { get; set; }
        public ReportSummaryViewModel Summary { get; set; }
        public IEnumerable<StockEntryReportViewModel> StockEntries { get; set; }
        public IEnumerable<ProductVariantReportViewModel> ProductVariants { get; set; }
        public IEnumerable<WarehouseStockReportViewModel> WarehouseStocks { get; set; }
        public Dictionary<string, List<ChartDataPoint>> ChartData { get; set; }

        public ReportsIndexViewModel()
        {
            Filters = new ReportFilterViewModel();
            Summary = new ReportSummaryViewModel();
            StockEntries = new List<StockEntryReportViewModel>();
            ProductVariants = new List<ProductVariantReportViewModel>();
            WarehouseStocks = new List<WarehouseStockReportViewModel>();
            ChartData = new Dictionary<string, List<ChartDataPoint>>();
        }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public string Color { get; set; }
    }
}