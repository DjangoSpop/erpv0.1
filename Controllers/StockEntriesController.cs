using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace erpv0._1.Controllers
{
    public class StockEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
   
        private readonly ILogger<StockEntriesController> _logger;
        private readonly IMemoryCache _cache;

        public StockEntriesController( ILogger<StockEntriesController> logger, IMemoryCache cache, ApplicationDbContext context)
        {
            
            _context = context;
            _logger = logger;
            _cache = cache;
        }

     


        private async Task LoadViewBagData()
        {
            ViewBag.Products = await _context.Products
                .OrderBy(p => p.ProductName)
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
            .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true)
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToListAsync();
        }
        public async Task<IActionResult> Index(string searchTerm, int? warehouseId, int? productId)
        {
            try
            {
                var entries = await _context.StockEntries
                    .Include(s => s.Product)
                    .Include(s => s.Warehouse)
                    .ToListAsync();

                // Apply filtering efficiently before calling ToList()
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.Trim().ToLower();
                    entries = entries
                        .Where(s => s.BatchNumber.ToLower().Contains(searchTerm) ||
                              s.SupplierInvoice.ToLower().Contains(searchTerm))
                        .ToList();
                }

                if (warehouseId.HasValue)
                    entries = entries.Where(s => s.WarehouseId == warehouseId).ToList();

                if (productId.HasValue)
                    entries = entries.Where(s => s.ProductId == productId).ToList();

                await LoadViewBagData();
                return View(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock entries");
                TempData["Error"] = "حدث خطأ أثناء تحميل بيانات المخزون";
                return View(new List<StockEntry>());
            }
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View(new StockEntry
            {
                ReceiptDate = DateTime.Today,
               
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockEntry entry)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validate batch number uniqueness
                    if (await _context.StockEntries.AnyAsync(s => s.BatchNumber == entry.BatchNumber))
                    {
                        ModelState.AddModelError("BatchNumber", "رقم الدفعة مستخدم مسبقاً");
                        await LoadViewBagData();
                        return View(entry);
                    }

                    // Set audit fields
                    entry.CreatedAt = DateTime.UtcNow;
                    entry.CreatedBy = "System"; // Replace with actual user when auth is added
                    entry.UpdatedAt = DateTime.UtcNow;
                    entry.UpdatedBy = "System";

                    _context.Add(entry);
                    await _context.SaveChangesAsync();

                    // Create stock movement record
                    var movement = new StockMovement
                    {
                        ProductId = entry.ProductId,
                        StockEntryId = entry.EntryId,
                        MovementType = "IN",
                        Quantity = entry.Quantity,
                        MovementDate = DateTime.UtcNow,
                        Reference = $"Initial Stock Entry - {entry.BatchNumber}",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System",
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = "System"
                    };

                    _context.StockMovements.Add(movement);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "تم إضافة السجل بنجاح";
                    return RedirectToAction(nameof(Details), new { id = entry.EntryId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating stock entry");
                    ModelState.AddModelError("", "حدث خطأ أثناء حفظ البيانات");
                }
            }

            await LoadViewBagData();
            return View(entry);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _context.StockEntries
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.EntryId == id);

            if (entry == null)
            {
                return NotFound();
            }

            await LoadViewBagData();
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockEntry entry)
        {
            if (id != entry.EntryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEntry = await _context.StockEntries
                        .Include(s => s.StockMovements)
                        .FirstOrDefaultAsync(s => s.EntryId == id);

                    if (existingEntry == null)
                    {
                        return NotFound();
                    }

                    // Check if batch number changed and is unique
                    if (entry.BatchNumber != existingEntry.BatchNumber &&
                        await _context.StockEntries.AnyAsync(s => s.BatchNumber == entry.BatchNumber))
                    {
                        ModelState.AddModelError("BatchNumber", "رقم الدفعة مستخدم مسبقاً");
                        await LoadViewBagData();
                        return View(entry);
                    }

                    // Check if quantity changed
                    if (entry.Quantity != existingEntry.Quantity)
                    {
                        var quantityDifference = entry.Quantity - existingEntry.Quantity;

                        // Create adjustment movement
                        var movement = new StockMovement
                        {
                            ProductId = entry.ProductId,
                            StockEntryId = entry.EntryId,
                            MovementType = quantityDifference > 0 ? "IN" : "OUT",
                            Quantity = Math.Abs(quantityDifference),
                            MovementDate = DateTime.UtcNow,
                            Reference = "Quantity Adjustment",
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "System",
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = "System"
                        };

                        _context.StockMovements.Add(movement);
                    }

                    // Update entry fields
                    existingEntry.ProductId = entry.ProductId;
                    existingEntry.WarehouseId = entry.WarehouseId;
                    existingEntry.BatchNumber = entry.BatchNumber;
                    existingEntry.Quantity = entry.Quantity;
                    existingEntry.CostPrice = entry.CostPrice;
                    existingEntry.SellingPrice = entry.SellingPrice;
                    existingEntry.MaxDiscount = entry.MaxDiscount;
                    existingEntry.ReceiptDate = entry.ReceiptDate;
                    existingEntry.SupplierInvoice = entry.SupplierInvoice;
                    existingEntry.UpdatedAt = DateTime.UtcNow;
                    existingEntry.UpdatedBy = "System";

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث السجل بنجاح";
                    return RedirectToAction(nameof(Details), new { id = entry.EntryId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockEntryExists(entry.EntryId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating stock entry");
                    ModelState.AddModelError("", "حدث خطأ أثناء حفظ البيانات");
                }
            }

            await LoadViewBagData();
            return View(entry);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var entry = await _context.StockEntries
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.StockMovements)
                .FirstOrDefaultAsync(s => s.EntryId == id);

            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _context.StockEntries
                .Include(s => s.StockMovements)
                .FirstOrDefaultAsync(s => s.EntryId == id);

            if (entry == null)
            {
                return Json(new { success = false, message = "السجل غير موجود" });
            }

            if (entry.StockMovements.Count > 1)
            {
                return Json(new { success = false, message = "لا يمكن حذف السجل لوجود حركات مخزون مرتبطة به" });
            }

            try
            {
                _context.StockMovements.RemoveRange(entry.StockMovements);
                _context.StockEntries.Remove(entry);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock entry");
                return Json(new { success = false, message = "حدث خطأ أثناء حذف السجل" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBulk([FromBody] List<int> ids)
        {
            try
            {
                var entries = await _context.StockEntries
                    .Include(s => s.StockMovements)
                    .Where(s => ids.Contains(s.EntryId))
                    .ToListAsync();

                // Check if any entry has multiple movements
                if (entries.Any(e => e.StockMovements.Count > 1))
                {
                    return Json(new { success = false, message = "بعض السجلات لا يمكن حذفها لوجود حركات مرتبطة" });
                }

                foreach (var entry in entries)
                {
                    _context.StockMovements.RemoveRange(entry.StockMovements);
                    _context.StockEntries.Remove(entry);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk deleting stock entries");
                return Json(new { success = false, message = "حدث خطأ أثناء حذف السجلات" });
            }
        }

        private bool StockEntryExists(int id)
        {
            return _context.StockEntries.Any(e => e.EntryId == id);
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "لم يتم اختيار ملف" });
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount < 2)
                {
                    return Json(new { success = false, message = "الملف فارغ" });
                }

                var entries = new List<StockEntry>();
                var errors = new List<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var entry = new StockEntry
                        {
                            BatchNumber = worksheet.Cells[row, 1].GetValue<string>(),
                            ProductId = worksheet.Cells[row, 2].GetValue<int>(),
                            WarehouseId = worksheet.Cells[row, 3].GetValue<int>(),
                            Quantity = worksheet.Cells[row, 4].GetValue<int>(),
                            CostPrice = worksheet.Cells[row, 5].GetValue<decimal>(),
                            SellingPrice = worksheet.Cells[row, 6].GetValue<decimal>(),
                            ReceiptDate = worksheet.Cells[row, 7].GetValue<DateTime>(),
                            SupplierInvoice = worksheet.Cells[row, 8].GetValue<string>(),
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "System",
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = "System"
                        };

                        entries.Add(entry);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"خطأ في الصف {row}: {ex.Message}");
                    }
                }

                if (errors.Any())
                {
                    return Json(new { success = false, message = string.Join("\n", errors) });
                }

                foreach (var entry in entries)
                {
                    _context.StockEntries.Add(entry);

                    var movement = new StockMovement
                    {
                        ProductId = entry.ProductId,
                        StockEntryId = entry.EntryId,
                        MovementType = "IN",
                        Quantity = entry.Quantity,
                        MovementDate = DateTime.UtcNow,
                        Reference = $"Initial Stock Entry - {entry.BatchNumber}",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System",
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = "System"
                    };

                    _context.StockMovements.Add(movement);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"تم استيراد {entries.Count} سجل بنجاح" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing Excel file");
                return Json(new { success = false, message = "حدث خطأ أثناء استيراد الملف" });
            }
        }

        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var entries = await _context.StockEntries
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Stock Entries");

                // Headers
                worksheet.Cells["A1"].Value = "رقم الدفعة";
                worksheet.Cells["B1"].Value = "المنتج";
                worksheet.Cells["C1"].Value = "المستودع";
                worksheet.Cells["D1"].Value = "الكمية";
                worksheet.Cells["E1"].Value = "سعر التكلفة";
                worksheet.Cells["F1"].Value = "سعر البيع";
                worksheet.Cells["G1"].Value = "تاريخ الاستلام";
                worksheet.Cells["H1"].Value = "رقم الفاتورة";

                // Data
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    worksheet.Cells[i + 2, 1].Value = entry.BatchNumber;
                    worksheet.Cells[i + 2, 2].Value = entry.Product;
                    worksheet.Cells[i + 2, 3].Value = entry.WarehouseId; // Assuming WarehouseId is sufficient
                    worksheet.Cells[i + 2, 4].Value = entry.Quantity;
                    worksheet.Cells[i + 2, 5].Value = entry.CostPrice;
                    worksheet.Cells[i + 2, 6].Value = entry.SellingPrice;
                    worksheet.Cells[i + 2, 7].Value = entry.ReceiptDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 8].Value = entry.SupplierInvoice;
                }

                // Formatting
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                worksheet.Cells.AutoFitColumns();

                return File(
                    package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"StockEntries_{DateTime.Now:yyyyMMdd}.xlsx"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                TempData["Error"] = "حدث خطأ أثناء تصدير البيانات";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
