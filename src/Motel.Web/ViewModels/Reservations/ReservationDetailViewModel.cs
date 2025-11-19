using Motel.Domain.Enums;

namespace Motel.Web.ViewModels.Reservations;

/// <summary>
/// Comprehensive reservation detail view with all information
/// </summary>
public class ReservationDetailViewModel
{
    // Basic Information
    public Guid Id { get; set; }
    public string ClientName { get; set; } = default!;
    public string ClientPhone { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;

    // Dates
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string CheckInDateFormatted { get; set; } = default!;
    public string CheckOutDateFormatted { get; set; } = default!;
    public int NumberOfNights { get; set; }

    // Guest Information
    public int Guests { get; set; }

    // Pricing
    public decimal NightlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public string TotalAmountFormatted { get; set; } = default!;
    public decimal ExtraCharges { get; set; }
    public string ExtraChargesFormatted { get; set; } = default!;
    public decimal FinalTotal { get; set; }
    public string FinalTotalFormatted { get; set; } = default!;

    // Status
    public ReservationStatus Status { get; set; }
    public string StatusDisplay { get; set; } = default!;
    public string StatusBadgeClass { get; set; } = default!;

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public string CreatedAtFormatted { get; set; } = default!;
    public DateTime? ActualCheckInTime { get; set; }
    public string? ActualCheckInTimeFormatted { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public string? ActualCheckOutTimeFormatted { get; set; }

    // Notes
    public string? Notes { get; set; }

    // Action Permissions
    public bool CanCheckIn { get; set; }
    public bool CanCheckOut { get; set; }
    public bool CanCancel { get; set; }
    public bool CanEdit { get; set; }
    public bool CanIssueInvoice { get; set; }

    // Invoice Information (if exists)
    public Guid? InvoiceId { get; set; }
    public string? InvoiceSerial { get; set; }
}
