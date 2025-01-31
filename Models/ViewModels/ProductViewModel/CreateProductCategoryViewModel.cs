namespace ArabicWarehouseERP.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    namespace ArabicWarehouseERP.Models.ViewModels
    {
        public class CreateProductCategoryViewModel
        {
            [Required(ErrorMessage = "اسم الفئة مطلوب")]
            [StringLength(200, ErrorMessage = "اسم الفئة يجب أن يكون 200 حرف أو أقل")]
            [Display(Name = "اسم الفئة")]
            public string CategoryName { get; set; } = string.Empty;

            [StringLength(500, ErrorMessage = "الوصف يجب أن يكون 500 حرف أو أقل")]
            [Display(Name = "الوصف")]
            public string? Description { get; set; }

            [Display(Name = "نشط")]
            public bool IsActive { get; set; } = true;

            [Display(Name = "الترجمات")]
            public List<CategoryTranslationViewModel>? Translations { get; set; }

            // Additional metadata for categorization
            [Display(Name = "الفئة الأصلية")]
            public int? ParentCategoryId { get; set; }

            [Display(Name = "رمز التصنيف")]
            [StringLength(50, ErrorMessage = "رمز التصنيف يجب أن يكون 50 حرف أو أقل")]
            public string? CategoryCode { get; set; }

            // Validation for unique category name
            public bool ValidateUniqueCategoryName { get; set; } = true;
        }

        public class CategoryTranslationViewModel
        {
            [Required(ErrorMessage = "رمز اللغة مطلوب")]
            [StringLength(10, ErrorMessage = "رمز اللغة يجب أن يكون 10 حروف أو أقل")]
            [Display(Name = "رمز اللغة")]
            public string LanguageCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "اسم الفئة بالترجمة مطلوب")]
            [StringLength(200, ErrorMessage = "اسم الفئة بالترجمة يجب أن يكون 200 حرف أو أقل")]
            [Display(Name = "اسم الفئة المترجم")]
            public string TranslatedName { get; set; } = string.Empty;

            [StringLength(500, ErrorMessage = "الوصف المترجم يجب أن يكون 500 حرف أو أقل")]
            [Display(Name = "الوصف المترجم")]
            public string? TranslatedDescription { get; set; }
        }
    }
}
