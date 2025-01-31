namespace erpv0._1.Models;

public partial class PriceHistory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public DateTime EffectiveDate { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
