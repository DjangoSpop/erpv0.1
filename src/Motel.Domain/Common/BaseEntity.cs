namespace Motel.Domain.Common;

/// <summary>
/// Base entity class with audit fields for enterprise tracking
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Soft delete flag - records are never truly deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Creation timestamp (UTC)
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Last modification timestamp (UTC)
    /// </summary>
    public DateTime? ModifiedAtUtc { get; set; }

    /// <summary>
    /// User who last modified this record
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Concurrency token for optimistic locking
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
