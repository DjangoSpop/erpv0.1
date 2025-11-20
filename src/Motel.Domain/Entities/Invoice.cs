using System.ComponentModel.DataAnnotations;
using Motel.Domain.Common;
using Motel.Domain.Enums;

namespace Motel.Domain.Entities;

/// <summary>
/// Invoice entity
/// </summary>
public class Invoice : BaseEntity
{

    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = default!;

    public DateTime IssuedAtUtc { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public decimal Subtotal { get; set; }

    public decimal TaxPercent { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public DateTime? PaidAtUtc { get; set; }

    [Required]
    [MaxLength(50)]
    public string Serial { get; set; } = default!;

    /// <summary>
    /// Line items for detailed invoice breakdown
    /// </summary>
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}
