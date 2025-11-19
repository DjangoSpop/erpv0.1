using Microsoft.AspNetCore.Mvc;
using Motel.Application.Interfaces;
using Motel.Domain.Enums;
using Motel.Web.Extensions;
using Motel.Web.ViewModels.Invoices;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class InvoicesController : Controller
{
    private readonly IInvoicesService _invoicesService;

    public InvoicesController(IInvoicesService invoicesService)
    {
        _invoicesService = invoicesService;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        InvoiceStatus? filterByStatus,
        DateOnly? filterFromDate,
        DateOnly? filterToDate,
        PaymentMethod? filterByPaymentMethod)
    {
        var invoices = await _invoicesService.GetAllAsync();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            invoices = invoices.Where(i =>
                i.Serial.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                i.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                i.RoomNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (filterByStatus.HasValue)
        {
            invoices = invoices.Where(i => i.Status == filterByStatus.Value);
        }

        if (filterFromDate.HasValue)
        {
            invoices = invoices.Where(i => i.IssuedAtUtc.Date >= filterFromDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (filterToDate.HasValue)
        {
            invoices = invoices.Where(i => i.IssuedAtUtc.Date <= filterToDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (filterByPaymentMethod.HasValue)
        {
            invoices = invoices.Where(i => i.PaymentMethod == filterByPaymentMethod.Value);
        }

        var viewModel = invoices.ToListViewModel(
            searchTerm,
            filterByStatus,
            filterFromDate,
            filterToDate,
            filterByPaymentMethod);

        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var invoice = await _invoicesService.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        var viewModel = invoice.ToDetailViewModel();
        return View(viewModel);
    }

    public async Task<IActionResult> Print(Guid id)
    {
        var invoice = await _invoicesService.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        return View(invoice);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Issue(Guid reservationId)
    {
        try
        {
            var invoice = await _invoicesService.IssueAsync(reservationId);
            TempData["Success"] = "تم إصدار الفاتورة بنجاح";
            return RedirectToAction(nameof(Details), new { id = invoice.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", "Reservations", new { id = reservationId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(Guid id, PaymentMethod paymentMethod)
    {
        try
        {
            await _invoicesService.MarkAsPaidAsync(id, paymentMethod);
            TempData["Success"] = "تم تسجيل الدفع بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            await _invoicesService.CancelAsync(id);
            TempData["Success"] = "تم إلغاء الفاتورة بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
