namespace erpv0._1.Models;

public partial class Invoice
{
    public int InvoiceNo { get; set; }

    public int? SupplierId { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public int? OrderId { get; set; }

    public virtual SubOrder? Order { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Supplier? Supplier { get; set; }
}
