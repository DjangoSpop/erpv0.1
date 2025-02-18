using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class StockEntryViewModel
    {
        public int EntryId { get; set; }

        [Required(ErrorMessage = "Please select a product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please select a warehouse")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Batch number is required")]
        public string BatchNumber { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Cost price is required")]
        public decimal CostPrice { get; set; }

        [Required(ErrorMessage = "Selling price is required")]
        public decimal SellingPrice { get; set; }

        public decimal MaxDiscount { get; set; }

        [Required(ErrorMessage = "Receipt date is required")]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Supplier invoice is required")]
        public string SupplierInvoice { get; set; }

        public string? ProductName { get; set; }
        public string? WarehouseName { get; set; }
    }
}
