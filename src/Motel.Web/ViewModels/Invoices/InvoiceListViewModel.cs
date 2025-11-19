using Motel.Domain.Enums;

namespace Motel.Web.ViewModels.Invoices;

/// <summary>
/// ViewModel for displaying a list of invoices with filtering
/// </summary>
public class InvoiceListViewModel
{
    public IEnumerable<InvoiceListItemViewModel> Invoices { get; set; } = new List<InvoiceListItemViewModel>();

    // Filtering
    public string? SearchTerm { get; set; }
    public InvoiceStatus? FilterByStatus { get; set; }
    public DateOnly? FilterFromDate { get; set; }
    public DateOnly? FilterToDate { get; set; }
    public PaymentMethod? FilterByPaymentMethod { get; set; }

    // Statistics
    public int TotalInvoices { get; set; }
    public int IssuedCount { get; set; }
    public int PaidCount { get; set; }
    public int CancelledCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal PendingRevenue { get; set; }

    // Formatted statistics
    public string TotalRevenueFormatted => $"{TotalRevenue:N2} ج.م";
    public string PendingRevenueFormatted => $"{PendingRevenue:N2} ج.م";
}

public class InvoiceListItemViewModel
{
    public Guid Id { get; set; }
    public string Serial { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string CheckInDateFormatted { get; set; } = default!;
    public string CheckOutDateFormatted { get; set; } = default!;
    public DateTime IssuedAt { get; set; }
    public string IssuedAtFormatted { get; set; } = default!;
    public decimal Total { get; set; }
    public string TotalFormatted { get; set; } = default!;
    public InvoiceStatus Status { get; set; }
    public string StatusDisplay { get; set; } = default!;
    public string StatusBadgeClass { get; set; } = default!;
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentMethodDisplay { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaidAtFormatted { get; set; }

    // Action permissions
    public bool CanPay { get; set; }
    public bool CanCancel { get; set; }
    public bool CanPrint { get; set; }
}
