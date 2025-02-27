using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using erpv0._1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace erpv0._1.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ApplicationDbContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Payment/ByInvoice/5
        public async Task<IActionResult> ByInvoice(int? invoiceNo)
        {
            if (invoiceNo == null)
            {
                return NotFound();
            }

            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Supplier)
                    .Include(i => i.Payments)
                    .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo);

                if (invoice == null)
                {
                    return NotFound();
                }

                ViewBag.InvoiceNo = invoice.InvoiceNo;
                ViewBag.InvoiceTotal = invoice.TotalAmount ?? 0;
                ViewBag.SupplierName = invoice.Supplier?.Name ?? "غير محدد";
                ViewBag.PaidAmount = invoice.Payments.Sum(p => p.Amount ?? 0);
                ViewBag.RemainingAmount = (invoice.TotalAmount ?? 0) - ViewBag.PaidAmount;

                var payments = await _context.Payments
                    .Where(p => p.InvoiceNo == invoiceNo)
                    .ToListAsync();

                return View(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payments for invoice with ID {invoiceNo}");
                TempData["Error"] = "حدث خطأ أثناء جلب مدفوعات الفاتورة";
                return RedirectToAction("Details", "Invoice", new { id = invoiceNo });
            }
        }

        // GET: Payment/Create
        public async Task<IActionResult> Create(int? invoiceNo)
        {
            if (invoiceNo == null)
            {
                return NotFound();
            }

            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Payments)
                    .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo);

                if (invoice == null)
                {
                    return NotFound();
                }

                decimal paidAmount = invoice.Payments.Sum(p => p.Amount ?? 0);
                decimal remainingAmount = (invoice.TotalAmount ?? 0) - paidAmount;

                ViewBag.InvoiceNo = invoice.InvoiceNo;
                ViewBag.RemainingAmount = remainingAmount;
                ViewBag.PaymentMethods = new[]
                {
                    new { Value = "Cash", Text = "نقدي" },
                    new { Value = "CreditCard", Text = "بطاقة ائتمان" },
                    new { Value = "BankTransfer", Text = "تحويل بنكي" },
                    new { Value = "Check", Text = "شيك" }
                };

                var payment = new Payment
                {
                    InvoiceNo = invoice.InvoiceNo,
                    PaymentDate = DateTime.Today,
                    Amount = remainingAmount > 0 ? remainingAmount : 0,
                    PaymentStatus = "Pending"
                };

                return View(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error preparing payment creation for invoice with ID {invoiceNo}");
                TempData["Error"] = "حدث خطأ أثناء تحضير صفحة إضافة دفعة جديدة";
                return RedirectToAction("ByInvoice", new { invoiceNo });
            }
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var invoice = await _context.Invoices
                        .Include(i => i.Payments)
                        .FirstOrDefaultAsync(i => i.InvoiceNo == payment.InvoiceNo);

                    if (invoice == null)
                    {
                        return NotFound();
                    }

                    // Verify payment amount doesn't exceed remaining balance
                    decimal paidAmount = invoice.Payments.Sum(p => p.Amount ?? 0);
                    decimal remainingAmount = (invoice.TotalAmount ?? 0) - paidAmount;

                    if ((payment.Amount ?? 0) > remainingAmount && remainingAmount > 0)
                    {
                        ModelState.AddModelError("Amount", "مبلغ الدفع يتجاوز المبلغ المتبقي من الفاتورة");

                        ViewBag.InvoiceNo = invoice.InvoiceNo;
                        ViewBag.RemainingAmount = remainingAmount;
                        ViewBag.PaymentMethods = new[]
                        {
                            new { Value = "Cash", Text = "نقدي" },
                            new { Value = "CreditCard", Text = "بطاقة ائتمان" },
                            new { Value = "BankTransfer", Text = "تحويل بنكي" },
                            new { Value = "Check", Text = "شيك" }
                        };

                        return View(payment);
                    }

                    // For backwards compatibility with existing fields
                    if (payment.PaymentMethod == "Cash" || payment.PaymentMethod == "Check")
                    {
                        payment.ShipmentMethod = payment.PaymentMethod;
                    }
                    payment.ShipmentStatus = payment.PaymentStatus;
                    payment.ShipmentDate = payment.PaymentDate;

                    _context.Add(payment);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "تم إضافة الدفعة بنجاح";
                    return RedirectToAction(nameof(ByInvoice), new { invoiceNo = payment.InvoiceNo });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating payment");
                    ModelState.AddModelError("", "حدث خطأ أثناء إضافة الدفعة");
                }
            }

            ViewBag.InvoiceNo = payment.InvoiceNo;
            ViewBag.PaymentMethods = new[]
            {
                new { Value = "Cash", Text = "نقدي" },
                new { Value = "CreditCard", Text = "بطاقة ائتمان" },
                new { Value = "BankTransfer", Text = "تحويل بنكي" },
                new { Value = "Check", Text = "شيك" }
            };

            return View(payment);
        }

        // GET: Payment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var payment = await _context.Payments
                    .Include(p => p.InvoiceNoNavigation)
                    .FirstOrDefaultAsync(p => p.PaymentId == id);

                if (payment == null)
                {
                    return NotFound();
                }

                return View(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payment with ID {id} for delete");
                TempData["Error"] = "حدث خطأ أثناء تحضير صفحة حذف الدفعة";
                return RedirectToAction("Index", "Invoice");
            }
        }

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound();
                }

                int? invoiceNo = payment.InvoiceNo;

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم حذف الدفعة بنجاح";

                if (invoiceNo.HasValue)
                {
                    return RedirectToAction(nameof(ByInvoice), new { invoiceNo });
                }
                else
                {
                    return RedirectToAction("Index", "Invoice");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting payment with ID {id}");
                TempData["Error"] = "حدث خطأ أثناء حذف الدفعة";
                return RedirectToAction("Index", "Invoice");
            }
        }
    }
}