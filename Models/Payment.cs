namespace erpv0._1.Models;

public partial class Payment
{
    public int ShipmentId { get; set; }

    public int? PaymentId { get; set; }

    public DateOnly? ShipmentDate { get; set; }

    public string? ShipmentStatus { get; set; }

    public string? ShipmentMethod { get; set; }

    public int? InvoiceNo { get; set; }

    public virtual Invoice? InvoiceNoNavigation { get; set; }
}
