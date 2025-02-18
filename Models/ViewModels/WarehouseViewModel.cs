using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class WarehouseViewModel
    {
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "الكود مطلوب")]
        [Display(Name = "الكود")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "الموقع مطلوب")]
        [Display(Name = "الموقع")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "الحالة")]
        public bool IsActive { get; set; } = true;

        public int ProductCount { get; set; }
        public decimal TotalStock { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class WarehouseListViewModel
    {
        public List<WarehouseViewModel> Warehouses { get; set; } = new();
        public string? SearchTerm { get; set; }
        public bool? IsActiveFilter { get; set; }
        public PaginationInfo Pagination { get; set; } = new();
        public WarehouseStatistics Statistics { get; set; } = new();

   
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)ItemsPerPage);
    }

    public class WarehouseStatistics
    {
        public int TotalWarehouses { get; set; }
        public int ActiveWarehouses { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
    }

    public static class WarehouseMapper
    {
        public static WarehouseViewModel ToViewModel(this Warehouse warehouse)
        {
            return new WarehouseViewModel
            {
                WarehouseId = warehouse.WarehouseId,
                Code = warehouse.Code,
                Name = warehouse.Name,
                Location = warehouse.Location,
                IsActive = warehouse.IsActive ?? true,
                ProductCount = warehouse.StockEntries?.Count ?? 0,
                TotalStock = warehouse.StockEntries?.Sum(se => se.Quantity * se.CostPrice) ?? 0,
                LastUpdated = warehouse.UpdatedAt
            };
        }

        public static Warehouse ToEntity(this WarehouseViewModel viewModel, Warehouse? existing = null)
        {
            var warehouse = existing ?? new Warehouse();
            warehouse.Code = viewModel.Code;
            warehouse.Name = viewModel.Name;
            warehouse.Location = viewModel.Location;
            warehouse.IsActive = viewModel.IsActive;

            if (existing == null)
            {
                warehouse.CreatedAt = DateTime.UtcNow;
                warehouse.CreatedBy = "System"; // Replace with actual user when auth is implemented
            }

            warehouse.UpdatedAt = DateTime.UtcNow;
            warehouse.UpdatedBy = "System"; // Replace with actual user when auth is implemented

            return warehouse;
        }
    }
}