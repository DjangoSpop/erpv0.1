using erpv0._1.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
namespace erpv0._1
{
    public class UpdatePriceHistoryViewModels : CreatePriceHistoryViewModel
    {
        [Required(ErrorMessage = "معرف سجل السعر مطلوب")]
        [Display(Name = "معرف سجل السعر")]
        public int Id { get; set; }

        [Display(Name = "اسم المنتج")]
        public new string ProductName { get; set; } = string.Empty;

        [Display(Name = "المستخدم المحدث")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "تاريخ التحديث")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        // Additional validation for update scenario
        [Required(ErrorMessage = "يجب تحديد سبب التحديث")]
        [StringLength(500, ErrorMessage = "يجب أن يكون سبب التحديث 500 حرف أو أقل")]
        [Display(Name = "سبب التحديث")]
        public string UpdateReason { get; set; } = string.Empty;

        // Computed properties for additional context
        [Display(Name = "السعر السابق")]
        public decimal? PreviousPrice { get; set; }

        [Display(Name = "نسبة التغيير")]
        public decimal? PriceChangePercentage
        {
            get
            {
                if (PreviousPrice.HasValue && PreviousPrice.Value != 0)
                {
                    return Math.Round((Price - PreviousPrice.Value) / PreviousPrice.Value * 100, 2);
                }
                return null;
            }
        }
    }

}
