using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace erpv0._1.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ApplicationDbContext context, ILogger<InvoiceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Supplier)
                    .Include(i => i.Order)
                    .Include(i => i.Payments)
                    .Select(i => new InvoiceViewModel
                    {
                        InvoiceNo = i.InvoiceNo,
                        SupplierId = i.SupplierId,
                        SupplierName = i.Supplier != null ? i.Supplier.Name : "غير محدد",
                        InvoiceDate = i.InvoiceDate,
                        TotalAmount = i.TotalAmount,
                        Notes = i.Notes,
                        OrderId = i.OrderId,
                        OrderNumber = i.OrderId.HasValue ? i.OrderId.Value.ToString() : "غير مرتبط",
                        PaymentCount = i.Payments.Count,
                        PaidAmount = i.Payments.Sum(p => p.Amount ?? 0)
                    })
                    .ToListAsync();

                return View(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices");
                TempData["Error"] = "حدث خطأ أثناء جلب الفواتير";
                return View(new List<InvoiceViewModel>());
            }
        }

        // GET: Invoice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Supplier)
                    .Include(i => i.Order)
                    .Include(i => i.Payments)
                    .Where(i => i.InvoiceNo == id)
                    .Select(i => new InvoiceViewModel
                    {
                        InvoiceNo = i.InvoiceNo,
                        SupplierId = i.SupplierId,
                        SupplierName = i.Supplier != null ? i.Supplier.Name : "غير محدد",
                        InvoiceDate = i.InvoiceDate,
                        TotalAmount = i.TotalAmount,
                        Notes = i.Notes,
                        OrderId = i.OrderId,
                        OrderNumber = i.OrderId.HasValue ? i.OrderId.Value.ToString() : "غير مرتبط",
                        PaymentCount = i.Payments.Count,
                        PaidAmount = i.Payments.Sum(p => p.Amount ?? 0)
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound();
                }

                // Get payments for display
                ViewBag.Payments = await _context.Payments
                    .Where(p => p.InvoiceNo == id)
                    .Select(p => new
                    {
                        p.PaymentId,
                        p.Amount,
                        p.PaymentDate,
                        p.PaymentMethod,
                        p.PaymentStatus
                    })
                    .ToListAsync();

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoice with ID {id}");
                TempData["Error"] = "حدث خطأ أثناء جلب تفاصيل الفاتورة";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Invoice/Create
        public IActionResult Create()
        {
            try
            {
                PopulateDropDownLists();
                return View(new InvoiceViewModel
                {
                    InvoiceDate = DateOnly.FromDateTime(DateTime.Today)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing invoice creation page");
                TempData["Error"] = "حدث خطأ أثناء تحضير صفحة إنشاء فاتورة جديدة";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var invoice = new Invoice
                    {
                        SupplierId = model.SupplierId,
                        InvoiceDate = model.InvoiceDate,
                        TotalAmount = model.TotalAmount,
                        Notes = model.Notes,
                        OrderId = model.OrderId
                    };

                    _context.Add(invoice);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "تم إنشاء الفاتورة بنجاح";
                    return RedirectToAction(nameof(Details), new { id = invoice.InvoiceNo });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating invoice");
                    ModelState.AddModelError("", "حدث خطأ أثناء إنشاء الفاتورة");
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Invoice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var invoice = await _context.Invoices
                    .Where(i => i.InvoiceNo == id)
                    .Select(i => new InvoiceViewModel
                    {
                        InvoiceNo = i.InvoiceNo,
                        SupplierId = i.SupplierId,
                        InvoiceDate = i.InvoiceDate,
                        TotalAmount = i.TotalAmount,
                        Notes = i.Notes,
                        OrderId = i.OrderId
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound();
                }

                PopulateDropDownLists(invoice);
                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoice with ID {id} for edit");
                TempData["Error"] = "حدث خطأ أثناء تحميل الفاتورة للتعديل";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceViewModel model)
        {
            if (id != model.InvoiceNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var invoice = await _context.Invoices.FindAsync(id);
                    if (invoice == null)
                    {
                        return NotFound();
                    }

                    invoice.SupplierId = model.SupplierId;
                    invoice.InvoiceDate = model.InvoiceDate;
                    invoice.TotalAmount = model.TotalAmount;
                    invoice.Notes = model.Notes;
                    invoice.OrderId = model.OrderId;

                    _context.Update(invoice);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "تم تحديث الفاتورة بنجاح";
                    return RedirectToAction(nameof(Details), new { id = invoice.InvoiceNo });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InvoiceExists(model.InvoiceNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, $"Concurrency error updating invoice with ID {id}");
                        ModelState.AddModelError("", "حدث خطأ في التزامن أثناء تحديث الفاتورة");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating invoice with ID {id}");
                    ModelState.AddModelError("", "حدث خطأ أثناء تحديث الفاتورة");
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Invoice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Supplier)
                    .Include(i => i.Order)
                    .Include(i => i.Payments)
                    .Where(i => i.InvoiceNo == id)
                    .Select(i => new InvoiceViewModel
                    {
                        InvoiceNo = i.InvoiceNo,
                        SupplierId = i.SupplierId,
                        SupplierName = i.Supplier != null ? i.Supplier.Name : "غير محدد",
                        InvoiceDate = i.InvoiceDate,
                        TotalAmount = i.TotalAmount,
                        Notes = i.Notes,
                        OrderId = i.OrderId,
                        OrderNumber = i.OrderId.HasValue ? i.OrderId.Value.ToString() : "غير مرتبط",
                        PaymentCount = i.Payments.Count,
                        PaidAmount = i.Payments.Sum(p => p.Amount ?? 0)
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound();
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoice with ID {id} for delete");
                TempData["Error"] = "حدث خطأ أثناء تحضير صفحة حذف الفاتورة";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Payments)
                    .FirstOrDefaultAsync(i => i.InvoiceNo == id);

                if (invoice == null)
                {
                    return NotFound();
                }

                // First remove all payments
                _context.Payments.RemoveRange(invoice.Payments);

                // Then remove the invoice
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم حذف الفاتورة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting invoice with ID {id}");
                TempData["Error"] = "حدث خطأ أثناء حذف الفاتورة";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX endpoint to get order total
        [HttpGet]
        public async Task<IActionResult> GetOrderTotal(int orderId)
        {
            try
            {
                var order = await _context.SubOrders.FindAsync(orderId);
                if (order == null)
                {
                    return NotFound();
                }

                // Replace this with actual logic to calculate the order total
                decimal total = order.TotalAmount ?? 0;

                return Json(new { success = true, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting total for order ID {orderId}");
                return Json(new { success = false, message = "حدث خطأ أثناء جلب قيمة الطلب" });
            }
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceNo == id);
        }

        private void PopulateDropDownLists(InvoiceViewModel model = null)
        {
            // Suppliers dropdown
            ViewBag.Suppliers = new SelectList(_context.Suppliers
                .OrderBy(s => s.Name),
                "SupplierId", "Name", model?.SupplierId);

            // Orders dropdown
            ViewBag.Orders = new SelectList(_context.SubOrders
                .OrderByDescending(o => o.OrderId),
                "OrderId", "OrderId", model?.OrderId);
        }
    }
}