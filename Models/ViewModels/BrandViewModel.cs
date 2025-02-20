using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels
{
    public class BrandViewModel
    {
        public int BrandId { get; set; }

        [Display(Name = "Brand Name")]
        [Required(ErrorMessage = "Brand Name is required.")]
        [StringLength(100, ErrorMessage = "Brand Name cannot be longer than 100 characters.")]
        public string BrandName { get; set; } = null!;
    }

    public class BrandViewModelArabic
    {
        public int BrandId { get; set; }

        [Display(Name = "اسم العلامة التجارية")]
        [Required(ErrorMessage = "اسم العلامة التجارية مطلوب.")]
        [StringLength(100, ErrorMessage = "اسم العلامة التجارية لا يمكن أن يكون أطول من 100 حرف.")]
        public string BrandName { get; set; } = null!;
    }
}
