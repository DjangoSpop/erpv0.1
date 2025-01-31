namespace erpv0._1.Models;

public partial class ProductVariant
{
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    public string Size { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string Style { get; set; } = null!;

    public decimal PriceAdjustment { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<StockEntry> StockEntries { get; set; } = new List<StockEntry>();
}
