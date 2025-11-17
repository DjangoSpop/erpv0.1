namespace Motel.Application.DTOs;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = default!;
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = default!;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Guests { get; set; }
    public bool CheckedIn { get; set; }
    public bool CheckedOut { get; set; }
    public decimal NightlyRate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ExtraCharges { get; set; }
    public string? Notes { get; set; }
    public int Nights { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateReservationDto
{
    public Guid ClientId { get; set; }
    public Guid RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Guests { get; set; }
    public decimal NightlyRate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ExtraCharges { get; set; }
    public string? Notes { get; set; }
}

public class CheckInDto
{
    public Guid ReservationId { get; set; }
}

public class CheckOutDto
{
    public Guid ReservationId { get; set; }
    public decimal? ExtraCharges { get; set; }
}
