using Motel.Domain.Enums;

namespace Motel.Application.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public string Serial { get; set; } = default!;

    // Related data for display
    public string ClientName { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
}

public class MarkPaidDto
{
    public Guid InvoiceId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
