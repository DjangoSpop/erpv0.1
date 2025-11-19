using Motel.Domain.Enums;

namespace Motel.Web.ViewModels.Invoices;

/// <summary>
/// Comprehensive invoice view with all details
/// </summary>
public class InvoiceDetailViewModel
{
    // Invoice Information
    public Guid Id { get; set; }
    public string Serial { get; set; } = default!;
    public DateTime IssuedAt { get; set; }
    public string IssuedAtFormatted { get; set; } = default!;
    public InvoiceStatus Status { get; set; }
    public string StatusDisplay { get; set; } = default!;
    public string StatusBadgeClass { get; set; } = default!;

    // Client Information
    public string ClientName { get; set; } = default!;
    public string ClientPhone { get; set; } = default!;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }

    // Reservation Information
    public Guid ReservationId { get; set; }
    public string RoomNumber { get; set; } = default!;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public int Guests { get; set; }

    // Line Items
    public List<InvoiceLineItem> LineItems { get; set; } = new();

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    // Formatted amounts (for display)
    public string SubtotalFormatted { get; set; } = default!;
    public string TaxAmountFormatted { get; set; } = default!;
    public string TotalFormatted { get; set; } = default!;

    // Payment Information
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentMethodDisplay { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaidAtFormatted { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }

    // Partial Payments
    public List<PaymentRecord> Payments { get; set; } = new();

    // Actions
    public bool CanEdit { get; set; }
    public bool CanPay { get; set; }
    public bool CanCancel { get; set; }
    public bool CanPrint { get; set; }
    public bool CanEmail { get; set; }
}

public class InvoiceLineItem
{
    public string Description { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string AmountFormatted { get; set; } = default!;
}

public class PaymentRecord
{
    public Guid Id { get; set; }
    public DateTime PaidAt { get; set; }
    public string PaidAtFormatted { get; set; } = default!;
    public decimal Amount { get; set; }
    public string AmountFormatted { get; set; } = default!;
    public PaymentMethod Method { get; set; }
    public string MethodDisplay { get; set; } = default!;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
