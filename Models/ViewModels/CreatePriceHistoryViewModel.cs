using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class CreatePriceHistoryViewModel
    {
        [Required(ErrorMessage = "المنتج مطلوب")]
        [Display(Name = "معرف المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Display(Name = "السعر")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر موجبًا")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "تاريخ التسعير مطلوب")]
        [Display(Name = "تاريخ التسعير")]
        [DataType(DataType.Date)]
        public DateTime PriceDate { get; set; }

        [StringLength(500, ErrorMessage = "الملاحظات يجب أن تكون 500 حرف أو أقل")]
        [Display(Name = "الملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "مصدر التسعير")]
        [StringLength(100, ErrorMessage = "مصدر التسعير يجب أن يكون 100 حرف أو أقل")]
        public string? PriceSource { get; set; }

        [Display(Name = "نوع التسعير")]
        public PriceType PriceType { get; set; }

        // Computed properties
        [Display(Name = "اسم المنتج")]
        public string? ProductName { get; set; }
    }

    public enum PriceType
    {
        [Display(Name = "سعر شراء")]
        PurchasePrice,

        [Display(Name = "سعر بيع")]
        SellingPrice,

        [Display(Name = "سعر مخفض")]
        DiscountPrice,

        [Display(Name = "سعر الجملة")]
        WholesalePrice
    }
}
