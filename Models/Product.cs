using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erpv0._1.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم المنتج يجب أن يكون بين 2 و 100 حرف")]
    [Display(Name = "اسم المنتج")]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = "العلامة التجارية مطلوبة")]
    [Display(Name = "العلامة التجارية")]
    public int BrandId { get; set; }

    [Display(Name = "سنة الموديل")]
    [Range(2000, 2100, ErrorMessage = "سنة الموديل يجب أن تكون بين 2000 و 2100")]
    public short ModelYear { get; set; }

    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
    [Column(TypeName = "decimal(10, 2)")]
    [Display(Name = "السعر")]
    public decimal ListPrice { get; set; }

    [Display(Name = "الفئة")]
    public int? CategoryId { get; set; }

    [Display(Name = "صورة المنتج")]
    public byte[]? ImageData { get; set; }

    //public string? ImageContentType { get; set; }

    //public string? QRCodeData { get; set; }

    //public string? BarcodeData { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public virtual Brand Brand { get; set; } = null!;
    public virtual ProductCategory? Category { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<StockEntry> StockEntries { get; set; } = new List<StockEntry>();
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    public virtual ICollection<SubOrderItem> SubOrderItems { get; set; } = new List<SubOrderItem>();
    public virtual ICollection<WarehouseTransfer> WarehouseTransfers { get; set; } = new List<WarehouseTransfer>();
}