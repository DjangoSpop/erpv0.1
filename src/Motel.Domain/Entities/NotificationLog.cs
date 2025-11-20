using System.ComponentModel.DataAnnotations;
using Motel.Domain.Common;

namespace Motel.Domain.Entities;

/// <summary>
/// Notification log for tracking sent emails/SMS
/// </summary>
public class NotificationLog : BaseEntity
{

    public Guid? ReservationId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Channel { get; set; } = "Email";

    [Required]
    [MaxLength(200)]
    public string Recipient { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = default!;

    [Required]
    public string Body { get; set; } = default!;

    public DateTime SentAtUtc { get; set; }

    public bool Success { get; set; }

    [MaxLength(1000)]
    public string? Error { get; set; }
}
