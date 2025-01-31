using erpv0._1.Models.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels.Warehouses
{
    public class WarehouseReportViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "يجب تحديد المستودع")]
        [Display(Name = "المستودع")]
        public required WarehouseViewModel Warehouse { get; set; }

        [Display(Name = "إجمالي الأصناف")]
        public int TotalProducts { get; set; }

        [Display(Name = "القيمة الإجمالية")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalValue { get; set; }

        [Display(Name = "مخزون منخفض")]
        public int LowStockProducts { get; set; }

        [Display(Name = "نفاذ المخزون")]
        public int OutOfStockProducts { get; set; }

        public List<WarehouseReportViewModel> Products { get; set; } = new();
        public List<StockMovementViewModel> Movements { get; set; } = new();
        public required StockDistributionViewModel StockDistribution { get; set; }
        public required ChartDataViewModel ChartData { get; set; }
    }

    // Additional supporting classes
    public class StockMovementViewModel
    {
        public DateTime Date { get; set; }
        public required string Type { get; set; }
        public int Quantity { get; set; }
        public required string Reference { get; set; }
        public decimal Value { get; set; }
        public required string UserName { get; set; }
    }

    public class StockDistributionViewModel
    {
        public int OptimalStock { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public List<CategoryDistribution> Categories { get; set; } = new();
    }

    public class CategoryDistribution
    {
        public required string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ChartDataViewModel
    {
        public List<StockMovementListViewModel> MovementData { get; set; } = new();
        public List<CategoryDistribution> DistributionData { get; set; } = new();
    }


    public class StockMovementListViewModel
    {
        public required string Date { get; set; }
        public int Quantity { get; set; }
    }
}
