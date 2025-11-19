using Motel.Application.DTOs;
using Motel.Domain.Enums;
using Motel.Web.ViewModels.Invoices;

namespace Motel.Web.Extensions;

/// <summary>
/// Extension methods for mapping Invoice DTOs to ViewModels
/// </summary>
public static class InvoiceMappingExtensions
{
    public static InvoiceListItemViewModel ToListItemViewModel(this InvoiceDto dto)
    {
        return new InvoiceListItemViewModel
        {
            Id = dto.Id,
            Serial = dto.Serial,
            ClientName = dto.ClientName,
            RoomNumber = dto.RoomNumber,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            CheckInDateFormatted = dto.CheckInDate.ToString("dd/MM/yyyy"),
            CheckOutDateFormatted = dto.CheckOutDate.ToString("dd/MM/yyyy"),
            IssuedAt = dto.IssuedAtUtc.ToLocalTime(),
            IssuedAtFormatted = dto.IssuedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            Total = dto.Total,
            TotalFormatted = $"{dto.Total:N2} ج.م",
            Status = dto.Status,
            StatusDisplay = GetInvoiceStatusDisplay(dto.Status),
            StatusBadgeClass = GetInvoiceStatusBadgeClass(dto.Status),
            PaymentMethod = dto.PaymentMethod,
            PaymentMethodDisplay = GetPaymentMethodDisplay(dto.PaymentMethod),
            PaidAt = dto.PaidAtUtc?.ToLocalTime(),
            PaidAtFormatted = dto.PaidAtUtc?.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            CanPay = dto.Status == InvoiceStatus.Issued,
            CanCancel = dto.Status is InvoiceStatus.Issued or InvoiceStatus.Draft,
            CanPrint = true
        };
    }

    public static InvoiceDetailViewModel ToDetailViewModel(this InvoiceDto dto)
    {
        var listItem = dto.ToListItemViewModel();

        return new InvoiceDetailViewModel
        {
            Id = listItem.Id,
            Serial = listItem.Serial,
            ClientName = listItem.ClientName,
            RoomNumber = listItem.RoomNumber,
            CheckInDate = listItem.CheckInDate,
            CheckOutDate = listItem.CheckOutDate,
            CheckInDateFormatted = listItem.CheckInDateFormatted,
            CheckOutDateFormatted = listItem.CheckOutDateFormatted,
            NumberOfNights = (listItem.CheckOutDate.ToDateTime(TimeOnly.MinValue) -
                             listItem.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days,
            IssuedAt = listItem.IssuedAt,
            IssuedAtFormatted = listItem.IssuedAtFormatted,
            Status = listItem.Status,
            StatusDisplay = listItem.StatusDisplay,
            StatusBadgeClass = listItem.StatusBadgeClass,

            // Line Items (will be populated from breakdown)
            LineItems = GenerateLineItems(dto),

            // Pricing
            Subtotal = dto.Subtotal,
            SubtotalFormatted = $"{dto.Subtotal:N2} ج.م",
            TaxPercent = dto.TaxPercent,
            TaxAmount = dto.TaxAmount,
            TaxAmountFormatted = $"{dto.TaxAmount:N2} ج.م",
            Total = dto.Total,
            TotalFormatted = $"{dto.Total:N2} ج.م",

            // Payment
            PaymentMethod = listItem.PaymentMethod,
            PaymentMethodDisplay = listItem.PaymentMethodDisplay,
            PaidAt = listItem.PaidAt,
            PaidAtFormatted = listItem.PaidAtFormatted,

            // Payment tracking (for future partial payments)
            Payments = new List<PaymentRecord>(),
            AmountPaid = dto.Status == InvoiceStatus.Paid ? dto.Total : 0,
            AmountDue = dto.Status == InvoiceStatus.Paid ? 0 : dto.Total,

            // Actions
            CanPay = listItem.CanPay,
            CanCancel = listItem.CanCancel,
            CanPrint = listItem.CanPrint
        };
    }

    public static InvoiceListViewModel ToListViewModel(
        this IEnumerable<InvoiceDto> invoices,
        string? searchTerm = null,
        InvoiceStatus? filterByStatus = null,
        DateOnly? filterFromDate = null,
        DateOnly? filterToDate = null,
        PaymentMethod? filterByPaymentMethod = null)
    {
        var invoiceItems = invoices.Select(i => i.ToListItemViewModel()).ToList();

        return new InvoiceListViewModel
        {
            Invoices = invoiceItems,
            SearchTerm = searchTerm,
            FilterByStatus = filterByStatus,
            FilterFromDate = filterFromDate,
            FilterToDate = filterToDate,
            FilterByPaymentMethod = filterByPaymentMethod,

            TotalInvoices = invoiceItems.Count,
            IssuedCount = invoiceItems.Count(i => i.Status == InvoiceStatus.Issued),
            PaidCount = invoiceItems.Count(i => i.Status == InvoiceStatus.Paid),
            CancelledCount = invoiceItems.Count(i => i.Status == InvoiceStatus.Cancelled),
            TotalRevenue = invoiceItems.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Total),
            PendingRevenue = invoiceItems.Where(i => i.Status == InvoiceStatus.Issued).Sum(i => i.Total)
        };
    }

    private static List<InvoiceLineItem> GenerateLineItems(InvoiceDto dto)
    {
        var items = new List<InvoiceLineItem>();

        // Calculate nights
        var nights = (dto.CheckOutDate.ToDateTime(TimeOnly.MinValue) -
                     dto.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;

        // Derive nightly rate from subtotal
        var nightlyRate = nights > 0 ? dto.Subtotal / nights : dto.Subtotal;

        // Add room charges line item
        items.Add(new InvoiceLineItem
        {
            Description = $"إقامة - غرفة {dto.RoomNumber}",
            Details = $"{nights} ليلة × {nightlyRate:N2} ج.م",
            Quantity = nights,
            UnitPrice = nightlyRate,
            Amount = dto.Subtotal,
            AmountFormatted = $"{dto.Subtotal:N2} ج.م"
        });

        // TODO: Add extra charges as separate line items when we enhance the data model

        return items;
    }

    private static string GetInvoiceStatusDisplay(InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Draft => "مسودة",
            InvoiceStatus.Issued => "صادرة",
            InvoiceStatus.Paid => "مدفوعة",
            InvoiceStatus.Cancelled => "ملغاة",
            InvoiceStatus.PartiallyPaid => "دفع جزئي",
            _ => status.ToString()
        };
    }

    private static string GetInvoiceStatusBadgeClass(InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Draft => "bg-secondary",
            InvoiceStatus.Issued => "bg-warning text-dark",
            InvoiceStatus.Paid => "bg-success",
            InvoiceStatus.Cancelled => "bg-danger",
            InvoiceStatus.PartiallyPaid => "bg-info",
            _ => "bg-secondary"
        };
    }

    private static string? GetPaymentMethodDisplay(PaymentMethod? method)
    {
        if (!method.HasValue) return null;

        return method.Value switch
        {
            PaymentMethod.Cash => "نقداً",
            PaymentMethod.Card => "بطاقة ائتمان",
            PaymentMethod.BankTransfer => "تحويل بنكي",
            PaymentMethod.MobileWallet => "محفظة إلكترونية",
            _ => method.ToString()
        };
    }
}
