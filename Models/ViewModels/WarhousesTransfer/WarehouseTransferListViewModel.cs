using Microsoft.AspNetCore.Mvc.Rendering;
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
        public WarehouseTransferCreateViewModel NewTransfer { get; set; } = new();
    }
    //public class WarehouseTransferCreateViewModel
    //{
    //    [Required(ErrorMessage = "المستودع المصدر مطلوب.")]
    //    [Display(Name = "المستودع المصدر")]
    //    public int SourceWarehouseId { get; set; }

    //    [Required(ErrorMessage = "المستودع الوجهة مطلوب.")]
    //    [Display(Name = "المستودع الوجهة")]
    //    public int DestWarehouseId { get; set; }

    //    [Required(ErrorMessage = "المنتج مطلوب.")]
    //    [Display(Name = "المنتج")]
    //    public int ProductId { get; set; }

    //    [Required(ErrorMessage = "الكمية مطلوبة.")]
    //    [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر.")]
    //    [Display(Name = "الكمية")]
    //    public int Quantity { get; set; }

    //    // This holds the list of available warehouses for the dropdowns.
    //    public List<SelectListItem> AvailableWarehouses { get; set; } = new List<SelectListItem>();

    //    // We don't need AvailableProducts here; it's handled by AJAX.
    //}
    public class WarehouseTransferCreateViewModel
    {
        [Display(Name = "المستودع المصدر")]
        [Required(ErrorMessage = "يجب اختيار المستودع المصدر")]
        public int SourceWarehouseId { get; set; }

        [Display(Name = "المستودع الوجهة")]
        [Required(ErrorMessage = "يجب اختيار المستودع الوجهة")]
        public int DestWarehouseId { get; set; }

        [Display(Name = "المنتج")]
        [Required(ErrorMessage = "يجب اختيار المنتج")]
        public int ProductId { get; set; }

        [Display(Name = "الكمية")]
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public int Quantity { get; set; }

        /// <summary>
        /// List of available warehouses for both source and destination dropdowns.
        /// </summary>
        public IEnumerable<SelectListItem> AvailableWarehouses { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// (Optional) List of available products. This list can be used to pre-populate the product dropdown 
        /// if you prefer not to load it via AJAX.
        /// </summary>
        public IEnumerable<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();
    }

    //}
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}
