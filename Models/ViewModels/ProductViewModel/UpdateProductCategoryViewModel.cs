
using ArabicWarehouseERP.Models.ViewModels.ArabicWarehouseERP.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels.ProductViewModel
{
    public class UpdateProductCategoryViewModel : CreateProductCategoryViewModel
    {
        [Required(ErrorMessage = "معرف الفئة مطلوب")]
        [Display(Name = "معرف الفئة")]
        public int Id { get; set; }

        [Display(Name = "تاريخ التحديث")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "محدث بواسطة")]
        [StringLength(100, ErrorMessage = "اسم المستخدم يجب أن يكون 100 حرف أو أقل")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "سبب التحديث")]
        [StringLength(500, ErrorMessage = "سبب التحديث يجب أن يكون 500 حرف أو أقل")]
        public string? UpdateReason { get; set; }



        [Display(Name = "عدد المنتجات")]
        public int ProductCount { get; set; }

        // Additional Metadata for Change Tracking
        [Display(Name = "التغييرات")]
        public List<CategoryChangeViewModel>? Changes { get; set; }
    }

    // Helper class to track changes
    public class CategoryChangeViewModel
    {
        [Display(Name = "الخاصية")]
        public string PropertyName { get; set; } = string.Empty;

        [Display(Name = "القيمة القديمة")]
        public string? OldValue { get; set; }

        [Display(Name = "القيمة الجديدة")]
        public string? NewValue { get; set; }

        [Display(Name = "تاريخ التغيير")]
        public DateTime ChangedAt { get; set; }
    }
}
