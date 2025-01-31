// Renaming the class to avoid conflict
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class ProductVariantIndexViewModelV2
    {
        // Pagination Properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        // Filtering Properties
        [Display(Name = "المنتج")]
        public int? ProductId { get; set; }

        [Display(Name = "المقاس")]
        public string Size { get; set; }

        [Display(Name = "اللون")]
        public string Color { get; set; }

        [Display(Name = "البحث")]
        public string SearchTerm { get; set; }

        // Data Collection
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

        // Dropdown Filters
        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colors { get; set; } = new List<SelectListItem>();

        // Computed Properties
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Summary Statistics
        public int TotalVariantsCount => ProductVariants.Count;
        public int ActiveVariantsCount => ProductVariants.Count(v => v.IsActive == true);
        public decimal AveragePriceAdjustment => ProductVariants.Any()
            ? ProductVariants.Average(v => v.PriceAdjustment)
            : 0;

        // Additional Filtering Options
        [Display(Name = "الحد الأدنى لتعديل السعر")]
        public decimal? MinPriceAdjustment { get; set; }

        [Display(Name = "الحد الأقصى لتعديل السعر")]
        public decimal? MaxPriceAdjustment { get; set; }

        // Sorting Options
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}