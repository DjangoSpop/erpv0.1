using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Domain.Enums;
using Motel.Infrastructure.Data;

namespace Motel.Infrastructure.Services;

public class InvoicesService : IInvoicesService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public InvoicesService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Reservation)
            .ThenInclude(r => r.Client)
            .Include(i => i.Reservation)
            .ThenInclude(r => r.Room)
            .Select(i => MapToDto(i))
            .ToListAsync();
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Reservation)
            .ThenInclude(r => r.Client)
            .Include(i => i.Reservation)
            .ThenInclude(r => r.Room)
            .FirstOrDefaultAsync(i => i.Id == id);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<InvoiceDto?> GetByReservationIdAsync(Guid reservationId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Reservation)
            .ThenInclude(r => r.Client)
            .Include(i => i.Reservation)
            .ThenInclude(r => r.Room)
            .FirstOrDefaultAsync(i => i.ReservationId == reservationId);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<InvoiceDto> IssueAsync(Guid reservationId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Room)
            .Include(r => r.Invoice)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
            throw new InvalidOperationException("الحجز غير موجود");

        // If invoice already exists and is issued, return it
        if (reservation.Invoice != null && reservation.Invoice.Status == InvoiceStatus.Issued)
            return MapToDto(reservation.Invoice);

        // Calculate amounts
        var nights = Math.Max(1, (reservation.CheckOutDate.DayNumber - reservation.CheckInDate.DayNumber));
        var subtotal = (reservation.NightlyRate * nights) + (reservation.ExtraCharges ?? 0) - (reservation.DiscountAmount ?? 0);

        var taxPercent = _configuration.GetValue<decimal>("Invoice:DefaultTaxPercent", 14);
        var taxAmount = subtotal * (taxPercent / 100);
        var total = subtotal + taxAmount;

        Invoice invoice;
        if (reservation.Invoice != null)
        {
            // Update existing draft invoice
            invoice = reservation.Invoice;
            invoice.Subtotal = subtotal;
            invoice.TaxPercent = taxPercent;
            invoice.TaxAmount = taxAmount;
            invoice.Total = total;
            invoice.IssuedAtUtc = DateTime.UtcNow;
            invoice.Status = InvoiceStatus.Issued;
        }
        else
        {
            // Create new invoice
            invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                ReservationId = reservationId,
                IssuedAtUtc = DateTime.UtcNow,
                Status = InvoiceStatus.Issued,
                Subtotal = subtotal,
                TaxPercent = taxPercent,
                TaxAmount = taxAmount,
                Total = total,
                Serial = await GenerateSerialAsync()
            };

            _context.Invoices.Add(invoice);
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(invoice.Id) ?? throw new InvalidOperationException();
    }

    public async Task<InvoiceDto> MarkAsPaidAsync(Guid invoiceId, PaymentMethod paymentMethod)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("الفاتورة غير موجودة");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("الفاتورة مدفوعة بالفعل");

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("الفاتورة ملغاة");

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaymentMethod = paymentMethod;
        invoice.PaidAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(invoiceId) ?? throw new InvalidOperationException();
    }

    public async Task<InvoiceDto> CancelAsync(Guid invoiceId)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("الفاتورة غير موجودة");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("لا يمكن إلغاء فاتورة مدفوعة");

        invoice.Status = InvoiceStatus.Cancelled;
        await _context.SaveChangesAsync();

        return await GetByIdAsync(invoiceId) ?? throw new InvalidOperationException();
    }

    public async Task<string> GenerateSerialAsync()
    {
        var now = DateTime.Now;
        var prefix = $"{now:yyyyMM}";

        // Get the last serial for this month
        var lastSerial = await _context.Invoices
            .Where(i => i.Serial.StartsWith(prefix))
            .OrderByDescending(i => i.Serial)
            .Select(i => i.Serial)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastSerial != null)
        {
            var lastNumberStr = lastSerial.Substring(prefix.Length + 1); // +1 for the dash
            if (int.TryParse(lastNumberStr, out var lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}-{nextNumber:D4}";
    }

    private static InvoiceDto MapToDto(Invoice i)
    {
        return new InvoiceDto
        {
            Id = i.Id,
            ReservationId = i.ReservationId,
            IssuedAtUtc = i.IssuedAtUtc,
            Status = i.Status,
            Subtotal = i.Subtotal,
            TaxPercent = i.TaxPercent,
            TaxAmount = i.TaxAmount,
            Total = i.Total,
            PaymentMethod = i.PaymentMethod,
            PaidAtUtc = i.PaidAtUtc,
            Serial = i.Serial,
            ClientName = i.Reservation.Client.FullName,
            RoomNumber = i.Reservation.Room.Number,
            CheckInDate = i.Reservation.CheckInDate,
            CheckOutDate = i.Reservation.CheckOutDate
        };
    }
}
