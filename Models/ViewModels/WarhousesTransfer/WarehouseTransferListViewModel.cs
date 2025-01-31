using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels.WarhousesTransfer
{
    public class WarehouseTransferListViewModel
    {
        // List of all transfers to display
        public IEnumerable<WarehouseTransfer> Transfers { get; set; } = new List<WarehouseTransfer>();

        // List of all available warehouses
        public IEnumerable<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        // List of products for filtering or dropdowns
        public List<Product> Products { get; set; } = new();

        // Search-related fields
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public int? SourceWarehouseId { get; set; }
        public int? DestWarehouseId { get; set; }

        // Pagination
        public PagingInfo PagingInfo { get; set; } = new PagingInfo();

        // Create a new transfer using the unified model
        public TransferCreateViewModel NewTransfer { get; set; } = new();
    }
    public class WarehouseTransferCreateViewModel
    {




        [Required(ErrorMessage = "يرجى اختيار المستودع المصدر")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المستودع الوجهة")]
        public int DestWarehouseId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "يرجى إدخال الكمية")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
        public int Quantity { get; set; }

        public string? Notes { get; set; }
        public List<Warehouse> AvailableWarehouses { get; set; } = new();
        public List<Product> AvailableProducts { get; set; } = new();
    }
    // Unified PagingInfo class
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }

    // Consolidated Create Transfer ViewModel
    public class TransferCreateViewModel
    {
        [Required(ErrorMessage = "يرجى اختيار المستودع المصدر")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المستودع الوجهة")]
        public int DestWarehouseId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "يرجى إدخال الكمية")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
        public int Quantity { get; set; }

        [MaxLength(500, ErrorMessage = "الملاحظات يجب ألا تتجاوز 500 حرف")]
        public string? Notes { get; set; }

        public List<Warehouse> AvailableWarehouses { get; set; } = new();
        public List<Product> AvailableProducts { get; set; } = new();
    }
}
