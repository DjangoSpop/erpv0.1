using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class ProductVariantViewModel
    {
        public int VariantId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المنتج")]
        [Display(Name = "المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "المقاس مطلوب")]
        [Display(Name = "المقاس")]
        [StringLength(50, ErrorMessage = "يجب أن يكون المقاس أقل من 50 حرفًا")]
        public string Size { get; set; }

        [Required(ErrorMessage = "اللون مطلوب")]
        [Display(Name = "اللون")]
        [StringLength(50, ErrorMessage = "يجب أن يكون اللون أقل من 50 حرفًا")]
        public string Color { get; set; }

        [Required(ErrorMessage = "النمط مطلوب")]
        [Display(Name = "النمط")]
        [StringLength(100, ErrorMessage = "يجب أن يكون النمط أقل من 100 حرفًا")]
        public string Style { get; set; }

        [Display(Name = "تعديل السعر")]
        [Range(-1000, 1000, ErrorMessage = "يجب أن يكون تعديل السعر بين -1000 و 1000")]
        public decimal PriceAdjustment { get; set; }

        [Display(Name = "نشط")]
        public bool? IsActive { get; set; }

        // Audit Trail Properties
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        // Dropdown Population Properties
        public List<SelectListItem> Products { get; set; }

        // Optional: Constructor to initialize collections
        public ProductVariantViewModel()
        {
            Products = new List<SelectListItem>();
            IsActive = true; // Default to active
        }
    }

    // Additional View Models for Product Variant Operations
    public class ProductVariantBulkImportViewModel
    {
        [Required(ErrorMessage = "الملف مطلوب")]
        [Display(Name = "ملف الاستيراد")]
        public IFormFile ImportFile { get; set; }

        [Display(Name = "استبدال المتغيرات الموجودة")]
        public bool ReplaceExisting { get; set; } = false;

        // Import Validation Results
        public int TotalImported { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }

    // View Model for Product Variant Index
    public class ProductVariantIndexViewModel
    {
        // Pagination Properties
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public int? ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }

        // Data Collection
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

        // Dropdown Filters
        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colors { get; set; } = new List<SelectListItem>();

        // Computed Properties
    }
}
