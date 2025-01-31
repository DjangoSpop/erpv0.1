using erpv0._1.Models.ViewModels.Base;
using System.ComponentModel.DataAnnotations;
namespace erpv0._1.Models.ViewModels.Warehouses
{
    public class WarehouseListViewModel : BaseViewModel
    {
        public List<Warehouse> Warehouses { get; set; } = new();
        public string SearchTerm { get; set; }
        public bool? IsActiveFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalWarehouses { get; set; }
        public int ActiveWarehouses { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        // Add this new property
        public WarehouseViewModel WarehouseModel { get; set; } = new WarehouseViewModel();
    }

    public class WarehouseViewModel : BaseViewModel
    {
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "الكود مطلوب")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(200)]
        public string Location { get; set; }

        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }
}
