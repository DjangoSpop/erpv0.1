using System.ComponentModel.DataAnnotations;
using Motel.Domain.Common;

namespace Motel.Domain.Entities;

/// <summary>
/// Invoice line item for detailed billing breakdown
/// </summary>
public class InvoiceLineItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = default!;

    /// <summary>
    /// Quantity (e.g., number of nights, number of items)
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Discount percentage applied to this line item
    /// </summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>
    /// Calculated line total (Quantity * UnitPrice * (1 - DiscountPercent/100))
    /// </summary>
    public decimal LineTotal { get; set; }

    /// <summary>
    /// Line item type (e.g., "Accommodation", "Extra Charges", "Services")
    /// </summary>
    [MaxLength(50)]
    public string? ItemType { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
