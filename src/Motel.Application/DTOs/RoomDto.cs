using Motel.Domain.Enums;

namespace Motel.Application.DTOs;

public class RoomDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = default!;
    public RoomType Type { get; set; }
    public decimal BaseNightlyRate { get; set; }
    public RoomStatus Status { get; set; }
    public int Capacity { get; set; }
    public string? Notes { get; set; }
}

public class CreateRoomDto
{
    public string Number { get; set; } = default!;
    public RoomType Type { get; set; }
    public decimal BaseNightlyRate { get; set; }
    public int Capacity { get; set; }
    public string? Notes { get; set; }
}

public class UpdateRoomDto
{
    public string Number { get; set; } = default!;
    public RoomType Type { get; set; }
    public decimal BaseNightlyRate { get; set; }
    public RoomStatus Status { get; set; }
    public int Capacity { get; set; }
    public string? Notes { get; set; }
}
