using Microsoft.AspNetCore.Mvc;
using Motel.Application.Interfaces;
using Motel.Domain.Enums;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class InvoicesController : Controller
{
    private readonly IInvoicesService _invoicesService;

    public InvoicesController(IInvoicesService invoicesService)
    {
        _invoicesService = invoicesService;
    }

    public async Task<IActionResult> Index()
    {
        var invoices = await _invoicesService.GetAllAsync();
        return View(invoices);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var invoice = await _invoicesService.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        return View(invoice);
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
