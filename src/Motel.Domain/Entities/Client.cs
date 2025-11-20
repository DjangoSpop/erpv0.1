using System.ComponentModel.DataAnnotations;
using Motel.Domain.Common;

namespace Motel.Domain.Entities;

/// <summary>
/// Guest/Client entity
/// </summary>
public class Client : BaseEntity
{

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string NationalIdOrPassport { get; set; } = default!;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = default!;

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
