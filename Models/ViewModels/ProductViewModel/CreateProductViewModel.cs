using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Models.ViewModels.ProductViewModel
{
    public class CreateProductViewModel
    {
        public CreateProductViewModel()
        {
            Name = string.Empty;
            SKU = string.Empty;
            Description = string.Empty;
            Barcode = string.Empty;
            QRCode = string.Empty;
        }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(200, ErrorMessage = "اسم المنتج يجب أن لا يتجاوز 200 حرف")]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رمز المنتج مطلوب")]
        [StringLength(50, ErrorMessage = "رمز المنتج يجب أن لا يتجاوز 50 حرف")]
        [Display(Name = "رمز المنتج")]
        public string SKU { get; set; }

        [Display(Name = "الوصف")]
        [MaxLength(500, ErrorMessage = "الوصف يجب أن لا يتجاوز 500 حرف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        [DataType(DataType.Currency)]
        [Display(Name = "السعر")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الحد الأدنى للمخزون مطلوب")]
        [Range(0, int.MaxValue, ErrorMessage = "الحد الأدنى للمخزون يجب أن يكون أكبر من أو يساوي صفر")]
        [Display(Name = "الحد الأدنى للمخزون")]
        public int MinimumStockLevel { get; set; }

        [Display(Name = "الباركود")]
        [StringLength(100, ErrorMessage = "الباركود يجب أن لا يتجاوز 100 حرف")]
        public string Barcode { get; set; }

        [Display(Name = "رمز QR")]
        [StringLength(100, ErrorMessage = "رمز QR يجب أن لا يتجاوز 100 حرف")]
        public string QRCode { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "الفئات")]
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
