using System.ComponentModel.DataAnnotations;
using Motel.Domain.Enums;

namespace Motel.Domain.Entities;

/// <summary>
/// Room entity
/// </summary>
public class Room
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = default!;

    public RoomType Type { get; set; }

    public decimal BaseNightlyRate { get; set; }

    public RoomStatus Status { get; set; } = RoomStatus.Available;

    public int Capacity { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
