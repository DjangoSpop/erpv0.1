using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class OrderItemViewModel
    {
        [Display(Name = "رقم الطلب")]
        [Required(ErrorMessage = "رقم الطلب مطلوب")]
        public int OrderId { get; set; }

        [Display(Name = "رقم العنصر")]
        [Required(ErrorMessage = "رقم العنصر مطلوب")]
        public int ItemId { get; set; }

        [Display(Name = "رقم المنتج")]
        [Required(ErrorMessage = "رقم المنتج مطلوب")]
        public int ProductId { get; set; }

        [Display(Name = "اسم المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "الكمية")]
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
        public int Quantity { get; set; }

        [Display(Name = "سعر القائمة")]
        [Required(ErrorMessage = "سعر القائمة مطلوب")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من صفر")]
        public decimal ListPrice { get; set; }

        [Display(Name = "الخصم")]
        [Required(ErrorMessage = "الخصم مطلوب")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "لا يمكن أن يكون الخصم قيمة سالبة")]
        public decimal Discount { get; set; }

        [Display(Name = "المبلغ الإجمالي")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount => Quantity * (ListPrice - Discount);

        [Display(Name = "رقم الطلب الرئيسي")]
        public int OrderNumber { get; set; }
    }
}
