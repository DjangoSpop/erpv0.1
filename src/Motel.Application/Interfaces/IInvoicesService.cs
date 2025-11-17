using Motel.Application.DTOs;
using Motel.Domain.Enums;

namespace Motel.Application.Interfaces;

public interface IInvoicesService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto?> GetByIdAsync(Guid id);
    Task<InvoiceDto?> GetByReservationIdAsync(Guid reservationId);
    Task<InvoiceDto> IssueAsync(Guid reservationId);
    Task<InvoiceDto> MarkAsPaidAsync(Guid invoiceId, PaymentMethod paymentMethod);
    Task<InvoiceDto> CancelAsync(Guid invoiceId);
    Task<string> GenerateSerialAsync();
}
