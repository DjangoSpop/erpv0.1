namespace erpv0._1.Models;

public partial class SubOrder
{
    public int OrderId { get; set; }

    public DateOnly? PlacedDate { get; set; }

    public DateOnly? FulfilledDate { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? StaffId { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Staff? Staff { get; set; }

    public virtual ICollection<SubOrderItem> SubOrderItems { get; set; } = new List<SubOrderItem>();
}
