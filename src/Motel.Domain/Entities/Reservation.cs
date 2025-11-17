using System.ComponentModel.DataAnnotations;

namespace Motel.Domain.Entities;

/// <summary>
/// Reservation entity
/// </summary>
public class Reservation
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int Guests { get; set; }

    public bool CheckedIn { get; set; }

    public bool CheckedOut { get; set; }

    /// <summary>
    /// Rate snapshot at the time of booking
    /// </summary>
    public decimal NightlyRate { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? ExtraCharges { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public Invoice? Invoice { get; set; }
}
