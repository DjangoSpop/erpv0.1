using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Models.ViewModels;
using OfficeOpenXml.Style;
using OfficeOpenXml;

public class StockEntriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StockEntriesController> _logger;

    public StockEntriesController(ApplicationDbContext context, ILogger<StockEntriesController> logger)
    {
        _context = context;
        _logger = logger;
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
        var viewModel = new StockEntryViewModel
        {
            ReceiptDate = DateTime.Today,
            MaxDiscount = 0
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StockEntryViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Check for unique batch number
                if (await _context.StockEntries.AnyAsync(s => s.BatchNumber == viewModel.BatchNumber))
                {
                    ModelState.AddModelError("BatchNumber", "This batch number already exists");
                    await LoadViewBagData();
                    return View(viewModel);
                }

                // Map ViewModel to Domain Model
                var stockEntry = new StockEntry
                {
                    ProductId = viewModel.ProductId,
                    WarehouseId = viewModel.WarehouseId,
                    BatchNumber = viewModel.BatchNumber,
                    Quantity = viewModel.Quantity,
                    CostPrice = viewModel.CostPrice,
                    SellingPrice = viewModel.SellingPrice,
                    MaxDiscount = viewModel.MaxDiscount,
                    ReceiptDate = viewModel.ReceiptDate,
                    SupplierInvoice = viewModel.SupplierInvoice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System", // Replace with actual user when auth is implemented
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                };

                _context.Add(stockEntry);
                await _context.SaveChangesAsync();

                // Create initial stock movement
                var movement = new StockMovement
                {
                    ProductId = stockEntry.ProductId,
                    StockEntryId = stockEntry.EntryId,
                    MovementType = "IN",
                    Quantity = stockEntry.Quantity,
                    MovementDate = DateTime.UtcNow,
                    Reference = $"Initial Stock Entry - {stockEntry.BatchNumber}",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                };

                _context.StockMovements.Add(movement);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Stock entry created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock entry");
                ModelState.AddModelError("", "An error occurred while saving the stock entry");
            }
        }

        await LoadViewBagData();
        return View(viewModel);
    }

    private async Task LoadViewBagData()
    {
        // Enhanced product selection with more details
        ViewBag.Products = await _context.Products
            .OrderBy(p => p.ProductName)
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = $"{p.ProductName} (Code: {p.ProductId})"
            })
            .ToListAsync();

        // Enhanced warehouse selection with more details
        ViewBag.Warehouses = await _context.Warehouses
            .Where(w => w.IsActive == true)
            .OrderBy(w => w.Name)
            .Select(w => new SelectListItem












            {
                Value = w.WarehouseId.ToString(),
                Text = $"{w.Name} ({w.Location})"
            })
            .ToListAsync();
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var stockEntry = await _context.StockEntries
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Include(s => s.StockMovements)
            .FirstOrDefaultAsync(s => s.EntryId == id);

        if (stockEntry == null)
        {
            return NotFound();
        }

        // Map domain model to view model
        var viewModel = new StockEntryViewModel
        {
            EntryId = stockEntry.EntryId,
            ProductId = stockEntry.ProductId,
            WarehouseId = stockEntry.WarehouseId,
            BatchNumber = stockEntry.BatchNumber,
            Quantity = stockEntry.Quantity,
            CostPrice = stockEntry.CostPrice,
            SellingPrice = stockEntry.SellingPrice,
            MaxDiscount = stockEntry.MaxDiscount,
            ReceiptDate = stockEntry.ReceiptDate,
            SupplierInvoice = stockEntry.SupplierInvoice,
            ProductName = stockEntry.Product.ProductName,
            WarehouseName = stockEntry.Warehouse.Name
        };

        await LoadViewBagData();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StockEntryViewModel viewModel)
    {
        if (id != viewModel.EntryId)
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

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Check if quantity changed
                    if (viewModel.Quantity != existingEntry.Quantity)
                    {
                        var quantityDifference = viewModel.Quantity - existingEntry.Quantity;

                        // Create adjustment movement
                        var movement = new StockMovement
                        {
                            ProductId = existingEntry.ProductId,
                            StockEntryId = existingEntry.EntryId,
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
                    existingEntry.ProductId = viewModel.ProductId;
                    existingEntry.WarehouseId = viewModel.WarehouseId;
                    existingEntry.BatchNumber = viewModel.BatchNumber;
                    existingEntry.Quantity = viewModel.Quantity;
                    existingEntry.CostPrice = viewModel.CostPrice;
                    existingEntry.SellingPrice = viewModel.SellingPrice;
                    existingEntry.MaxDiscount = viewModel.MaxDiscount;
                    existingEntry.ReceiptDate = viewModel.ReceiptDate;
                    existingEntry.SupplierInvoice = viewModel.SupplierInvoice;
                    existingEntry.UpdatedAt = DateTime.UtcNow;
                    existingEntry.UpdatedBy = "System";

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Stock entry updated successfully";
                    return RedirectToAction(nameof(Details), new { id = existingEntry.EntryId });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock entry");
                ModelState.AddModelError("", "An error occurred while updating the stock entry");
            }
        }

        await LoadViewBagData();
        return View(viewModel);
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
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var entry = await _context.StockEntries
    //        .Include(s => s.StockMovements)
    //        .FirstOrDefaultAsync(s => s.EntryId == id);

    //    if (entry == null)
    //    {
    //        return Json(new { success = false, message = "السجل غير موجود" });
    //    }

    //    if (entry.StockMovements.Count > 1)
    //    {
    //        return Json(new { success = false, message = "لا يمكن حذف السجل لوجود حركات مخزون مرتبطة به" });
    //    }

    //    try
    //    {
    //        _context.StockMovements.RemoveRange(entry.StockMovements);
    //        _context.StockEntries.Remove(entry);
    //        await _context.SaveChangesAsync();

    //        return Json(new { success = true });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error deleting stock entry");
    //        return Json(new { success = false, message = "حدث خطأ أثناء حذف السجل" });
    //    }
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteBulk([FromBody] List<int> ids)
    //{
    //    try
    //    {
    //        var entries = await _context.StockEntries
    //            .Include(s => s.StockMovements)
    //            .Where(s => ids.Contains(s.EntryId))
    //            .ToListAsync();

    //        // Check if any entry has multiple movements
    //        if (entries.Any(e => e.StockMovements.Count > 1))
    //        {
    //            return Json(new { success = false, message = "بعض السجلات لا يمكن حذفها لوجود حركات مرتبطة" });
    //        }

    //        foreach (var entry in entries)
    //        {
    //            _context.StockMovements.RemoveRange(entry.StockMovements);
    //            _context.StockEntries.Remove(entry);
    //        }

    //        await _context.SaveChangesAsync();
    //        return Json(new { success = true });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error bulk deleting stock entries");
    //        return Json(new { success = false, message = "حدث خطأ أثناء حذف السجلات" });
    //    }
    //}

    private bool StockEntryExists(int id)
    {
        return _context.StockEntries.Any(e => e.EntryId == id);
    }


    [HttpPost]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "يرجى تحميل ملف Excel صالح";
            return RedirectToAction("Index");
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            TempData["Error"] = "ملف Excel فارغ";
            return RedirectToAction("Index");
        }

        var rowCount = worksheet.Dimension.Rows;
        for (int row = 2; row <= rowCount; row++)
        {
            var batchNumber = worksheet.Cells[row, 1].Text;
            if (!int.TryParse(worksheet.Cells[row, 2].Text, out int quantity) ||
                !decimal.TryParse(worksheet.Cells[row, 3].Text, out decimal costPrice) ||
                !decimal.TryParse(worksheet.Cells[row, 4].Text, out decimal sellingPrice) ||
                !int.TryParse(worksheet.Cells[row, 5].Text, out int productId) ||
                !int.TryParse(worksheet.Cells[row, 6].Text, out int warehouseId))
            {
                TempData["Error"] = $"خطأ في البيانات بالصف {row}";
                return RedirectToAction("Index");
            }

            var stockEntry = new StockEntry
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                BatchNumber = batchNumber,
                Quantity = quantity,
                CostPrice = costPrice,
                SellingPrice = sellingPrice,
                ReceiptDate = DateTime.UtcNow,
                SupplierInvoice = "Auto-Generated",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedBy = "System"
            };

            _context.StockEntries.Add(stockEntry);
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "تم استيراد البيانات بنجاح";
        return RedirectToAction("Index");
    }

    // Export Excel File
    public async Task<IActionResult> ExportExcel()
    {
        var stockEntries = await _context.StockEntries.ToListAsync();
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("StockEntries");

        worksheet.Cells[1, 1].Value = "Batch Number";
        worksheet.Cells[1, 2].Value = "Quantity";
        worksheet.Cells[1, 3].Value = "Cost Price";
        worksheet.Cells[1, 4].Value = "Selling Price";
        worksheet.Cells[1, 5].Value = "Product ID";
        worksheet.Cells[1, 6].Value = "Warehouse ID";

        int row = 2;
        foreach (var entry in stockEntries)
        {
            worksheet.Cells[row, 1].Value = entry.BatchNumber;
            worksheet.Cells[row, 2].Value = entry.Quantity;
            worksheet.Cells[row, 3].Value = entry.CostPrice;
            worksheet.Cells[row, 4].Value = entry.SellingPrice;
            worksheet.Cells[row, 5].Value = entry.ProductId;
            worksheet.Cells[row, 6].Value = entry.WarehouseId;
            row++;
        }

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StockEntries.xlsx");
    }

    // Delete Single Entry
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var entry = await _context.StockEntries
                .Include(s => s.StockMovements) // Include related movements
                .FirstOrDefaultAsync(s => s.EntryId == id);

            if (entry == null)
            {
                TempData["Error"] = "العنصر غير موجود";
                return RedirectToAction("Index");
            }

            // Remove related StockMovements first
            _context.StockMovements.RemoveRange(entry.StockMovements);
            await _context.SaveChangesAsync();

            // Now, remove the StockEntry
            _context.StockEntries.Remove(entry);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Success"] = "تم حذف السجل بنجاح";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting stock entry: {Message}", ex.Message);
            TempData["Error"] = "حدث خطأ أثناء الحذف";
            return RedirectToAction("Index");
        }
    }

    // Bulk Delete Entries
    [HttpPost]
    public async Task<IActionResult> DeleteBulk([FromBody] List<int> ids)
    {
        var entries = await _context.StockEntries.Where(e => ids.Contains(e.EntryId)).ToListAsync();
        if (!entries.Any())
        {
            return Json(new { success = false, message = "لم يتم العثور على العناصر" });
        }

        _context.StockEntries.RemoveRange(entries);
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "تم حذف السجلات المحددة بنجاح" });
    }

    // Index (with Filtering)
   
}


