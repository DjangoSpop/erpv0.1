using Motel.Domain.Enums;

namespace Motel.Web.ViewModels.Rooms;

/// <summary>
/// ViewModel for displaying a list of rooms with filtering
/// </summary>
public class RoomListViewModel
{
    public IEnumerable<RoomItemViewModel> Rooms { get; set; } = new List<RoomItemViewModel>();

    // Filtering
    public string? SearchTerm { get; set; }
    public RoomType? FilterByType { get; set; }
    public RoomStatus? FilterByStatus { get; set; }
    public int? MinCapacity { get; set; }
    public decimal? MaxPrice { get; set; }

    // Availability search
    public DateOnly? AvailabilityFromDate { get; set; }
    public DateOnly? AvailabilityToDate { get; set; }
    public int? RequiredGuests { get; set; }

    // Statistics
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int ReservedRooms { get; set; }
}

public class RoomItemViewModel
{
    public Guid Id { get; set; }
    public string Number { get; set; } = default!;
    public string TypeDisplay { get; set; } = default!;
    public RoomType Type { get; set; }
    public string StatusDisplay { get; set; } = default!;
    public RoomStatus Status { get; set; }
    public string StatusBadgeClass { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal BaseNightlyRate { get; set; }
    public string FormattedRate { get; set; } = default!;
    public string? Notes { get; set; }
    public bool IsAvailable { get; set; }
    public int ActiveReservations { get; set; }
}
