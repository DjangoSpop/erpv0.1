using Motel.Domain.Enums;

namespace Motel.Web.ViewModels.Reservations;

/// <summary>
/// ViewModel for displaying a list of reservations with filtering
/// </summary>
public class ReservationListViewModel
{
    public IEnumerable<ReservationListItemViewModel> Reservations { get; set; } = new List<ReservationListItemViewModel>();

    // Filtering
    public string? SearchTerm { get; set; }
    public ReservationStatus? FilterByStatus { get; set; }
    public DateOnly? FilterFromDate { get; set; }
    public DateOnly? FilterToDate { get; set; }
    public Guid? FilterByClient { get; set; }
    public Guid? FilterByRoom { get; set; }

    // Statistics
    public int TotalReservations { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CheckedInCount { get; set; }
    public int CheckedOutCount { get; set; }
    public int TodayCheckIns { get; set; }
    public int TodayCheckOuts { get; set; }
}

public class ReservationListItemViewModel
{
    public Guid Id { get; set; }
    public string ClientName { get; set; } = default!;
    public string ClientPhone { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string CheckInDateFormatted { get; set; } = default!;
    public string CheckOutDateFormatted { get; set; } = default!;
    public int NumberOfNights { get; set; }
    public int Guests { get; set; }
    public decimal NightlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public string TotalAmountFormatted { get; set; } = default!;
    public ReservationStatus Status { get; set; }
    public string StatusDisplay { get; set; } = default!;
    public string StatusBadgeClass { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string CreatedAtFormatted { get; set; } = default!;
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }

    // Action permissions
    public bool CanCheckIn { get; set; }
    public bool CanCheckOut { get; set; }
    public bool CanCancel { get; set; }
}
