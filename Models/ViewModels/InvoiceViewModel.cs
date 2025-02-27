using System;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class InvoiceViewModel
    {
        [Display(Name = "رقم الفاتورة")]
        public int InvoiceNo { get; set; }

        [Display(Name = "المورد")]
        [Required(ErrorMessage = "الرجاء اختيار المورد")]
        public int? SupplierId { get; set; }

        [Display(Name = "اسم المورد")]
        public string SupplierName { get; set; } = "غير محدد";

        [Display(Name = "تاريخ الفاتورة")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "الرجاء تحديد تاريخ الفاتورة")]
        public DateOnly? InvoiceDate { get; set; }

        [Display(Name = "المبلغ الإجمالي")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "الرجاء إدخال المبلغ الإجمالي")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ أكبر من صفر")]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "ملاحظات")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "الملاحظات يجب ألا تتجاوز 500 حرف")]
        public string? Notes { get; set; }

        [Display(Name = "رقم الطلب")]
        public int? OrderId { get; set; }

        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; } = "غير مرتبط";

        [Display(Name = "عدد المدفوعات")]
        public int PaymentCount { get; set; }

        [Display(Name = "المبلغ المدفوع")]
        [DataType(DataType.Currency)]
        public decimal PaidAmount { get; set; }

        [Display(Name = "المبلغ المتبقي")]
        [DataType(DataType.Currency)]
        public decimal RemainingAmount => (TotalAmount ?? 0) - PaidAmount;

        [Display(Name = "حالة الدفع")]
        public string PaymentStatus => GetPaymentStatus();

        private string GetPaymentStatus()
        {
            if (!TotalAmount.HasValue || TotalAmount.Value == 0)
                return "غير محدد";

            if (PaidAmount == 0)
                return "غير مدفوع";

            if (PaidAmount >= TotalAmount.Value)
                return "مدفوع بالكامل";

            return "مدفوع جزئياً";
        }
    }
}