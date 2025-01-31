namespace erpv0.Models.ViewModels
{
    using global::erpv0._1.Models.ViewModels;
    // ViewModels/ProductViewModel.cs
    using System.ComponentModel.DataAnnotations;

    namespace erpv0.ViewModels
    {
        public class ProductViewModel
        {
            public ProductViewModel()
            {
                Name = string.Empty;
                SKU = string.Empty;
                Barcode = string.Empty;
                QRCode = string.Empty;
            }

            public int Id { get; set; }

            [Required(ErrorMessage = "اسم المنتج مطلوب")]
            [StringLength(200, ErrorMessage = "اسم المنتج يجب أن لا يتجاوز 200 حرف")]
            [Display(Name = "اسم المنتج")]
            public string Name { get; set; }

            [Required(ErrorMessage = "رمز المنتج مطلوب")]
            [StringLength(50, ErrorMessage = "رمز المنتج يجب أن لا يتجاوز 50 حرف")]
            [Display(Name = "رمز المنتج")]
            public string SKU { get; set; }

            [Required(ErrorMessage = "السعر مطلوب")]
            [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
            [DataType(DataType.Currency)]
            [Display(Name = "السعر")]
            public decimal Price { get; set; }

            [Display(Name = "الكمية المتوفرة")]
            public int StockQuantity { get; set; }

            [Display(Name = "الحد الأدنى للمخزون")]
            [Range(0, int.MaxValue, ErrorMessage = "الحد الأدنى للمخزون يجب أن يكون أكبر من أو يساوي صفر")]
            public int MinimumStockLevel { get; set; }

            [Display(Name = "الباركود")]
            [StringLength(100)]
            public string Barcode { get; set; }

            [Display(Name = "رمز QR")]
            [StringLength(100)]
            public string QRCode { get; set; }

            [Display(Name = "فعال")]
            public bool IsActive { get; set; }

            [Display(Name = "القيمة الإجمالية")]
            [DisplayFormat(DataFormatString = "{0:N2}")]
            public decimal TotalValue => Price * StockQuantity;

            [Display(Name = "حالة المخزون")]
            public string StockStatus => StockQuantity <= MinimumStockLevel ? "منخفض" : "طبيعي";

            [Display(Name = "تاريخ الإنشاء")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
            public DateTime CreatedAt { get; set; }

            [Display(Name = "تم الإنشاء بواسطة")]
            public string CreatedBy { get; set; } = string.Empty;

            [Display(Name = "آخر تحديث")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
            public DateTime? UpdatedAt { get; set; }

            [Display(Name = "تم التحديث بواسطة")]
            public string? UpdatedBy { get; set; }
        }

        public class ProductListViewModel
        {
            public ProductListViewModel()
            {
                Products = new List<ProductViewModel>();
            }

            public List<ProductViewModel> Products { get; set; }
            public string SearchTerm { get; set; } = string.Empty;
            public bool ShowInactive { get; set; }
            public bool ShowLowStock { get; set; }

            [Display(Name = "إجمالي المنتجات")]
            public int TotalProducts => Products.Count;

            [Display(Name = "إجمالي القيمة")]
            [DisplayFormat(DataFormatString = "{0:N2}")]
            public decimal TotalValue => Products.Sum(p => p.TotalValue);
        }

        public class ProductStockViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public Dictionary<string, int> StockByWarehouse { get; set; } = new();
            public int TotalStock { get; set; }
            public decimal TotalValue { get; set; }
            public List<StockMovementViewModel> RecentMovements { get; set; } = new();
        }

        public class SelectProductViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string SKU { get; set; } = string.Empty;
            public int StockQuantity { get; set; }
            public decimal Price { get; set; }
            public bool IsLowStock { get; set; }
        }
    }
}
