// ViewModels/WarehouseTransferViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels.WarhousesTransfer
{
    public class WarehouseTransferViewModel
    {
        public int Id { get; set; }

        [Display(Name = "المنتج")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "المستودع المصدر")]
        public string SourceWarehouseName { get; set; } = string.Empty;

        [Display(Name = "المستودع الوجهة")]
        public string DestWarehouseName { get; set; } = string.Empty;

        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "تاريخ الطلب")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RequestedDate { get; set; }

        [Display(Name = "تاريخ الإكمال")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CompletionDate { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }

    public class CreateTransferViewModels
    {
        [Required(ErrorMessage = "المنتج مطلوب")]
        [Display(Name = "المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "المستودع المصدر مطلوب")]
        [Display(Name = "المستودع المصدر")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "المستودع الوجهة مطلوب")]
        [Display(Name = "المستودع الوجهة")]
        public int DestWarehouseId { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Display(Name = "ملاحظات")]
        [MaxLength(500, ErrorMessage = "الملاحظات يجب ألا تتجاوز 500 حرف")]
        public string? Notes { get; set; }
    }

    public class TransferListViewModel
    {
        public TransferListViewModel()
        {
            Transfers = new List<WarehouseTransferViewModel>();
        }

        public List<WarehouseTransferViewModel> Transfers { get; set; }
        public bool ShowCompleted { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}