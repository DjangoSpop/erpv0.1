// StockMovementsController.cs
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class StockMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockMovementsController> _logger;

        public StockMovementsController(ApplicationDbContext context,
            ILogger<StockMovementsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchTerm, string movementTypeFilter, int page = 1)
        {
            try
            {
                var query = _context.StockMovements
                    .Include(s => s.Product)
                    .Include(s => s.StockEntry)
                    .ThenInclude(se => se.Warehouse)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s =>
                        s.Product.ProductName.Contains(searchTerm) ||
                        s.Reference.Contains(searchTerm) ||
                        s.StockEntry.Warehouse.Name.Contains(searchTerm));
                }

                if (!string.IsNullOrEmpty(movementTypeFilter))
                {
                    query = query.Where(s => s.MovementType == movementTypeFilter);
                }

                int pageSize = 10;
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var movements = await query
                    .OrderByDescending(s => s.MovementDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new StockMovementListViewModel
                {
                    Movements = movements.Select(m => new StockMovementViewModel
                    {
                        MovementId = m.MovementId,
                        ProductName = m.Product.ProductName,
                        MovementType = m.MovementType,
                        Quantity = m.Quantity,
                        MovementDate = m.MovementDate,
                        Reference = m.Reference,
                        WarehouseName = m.StockEntry.Warehouse.Name,
                        CreatedBy = m.CreatedBy
                    }).ToList(),
                    TotalMovements = totalItems,
                    SearchTerm = searchTerm,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    IncomingMovements = await query.CountAsync(s => s.MovementType == "IN"),
                    OutgoingMovements = await query.CountAsync(s => s.MovementType == "OUT")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock movements");
                TempData["Error"] = "حدث خطأ أثناء تحميل حركات المخزون";
                return View(new StockMovementListViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockMovement movement)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(movement);
            }

            try
            {
                var stockEntry = await _context.StockEntries.FindAsync(movement.StockEntryId);
                if (stockEntry == null)
                {
                    ModelState.AddModelError("StockEntryId", "المخزون غير موجود");
                    await LoadViewBags();
                    return View(movement);
                }

                // Validate stock availability for OUT movements
                if (movement.MovementType == "OUT" && movement.Quantity > stockEntry.Quantity)
                {
                    ModelState.AddModelError("Quantity", "الكمية المراد سحبها تتجاوز المخزون المتاح");
                    await LoadViewBags();
                    return View(movement);
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    stockEntry.Quantity += movement.MovementType == "IN" ? movement.Quantity : -movement.Quantity;
                    movement.CreatedAt = DateTime.UtcNow;
                    movement.CreatedBy = User.Identity?.Name ?? "System";

                    _context.StockMovements.Add(movement);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "تم تسجيل حركة المخزون بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock movement");
                ModelState.AddModelError("", "حدث خطأ أثناء حفظ حركة المخزون");
                await LoadViewBags();
                return View(movement);
            }
        }
        public async Task<IActionResult> GetStockMovementAnalytics(int productId, DateTime startDate, DateTime endDate)
        {
            var movements = await _context.StockMovements
                .Where(m => m.ProductId == productId &&
                           m.MovementDate >= startDate &&
                           m.MovementDate <= endDate)
                .GroupBy(m => new { m.MovementType, Date = m.MovementDate.Date })
                .Select(g => new {
                    Date = g.Key.Date,
                    Type = g.Key.MovementType,
                    Quantity = g.Sum(m => m.Quantity)
                })
                .ToListAsync();

            return Json(movements);
        }
        private async Task LoadViewBags()
        {
            ViewBag.Products = await _context.Products
                .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.ProductName })
                .ToListAsync();

            ViewBag.StockEntries = await _context.StockEntries
                .Include(s => s.Warehouse)
                .Select(s => new SelectListItem
                {
                    Value = s.EntryId.ToString(),
                    Text = $"{s.Warehouse.Name} - {s.BatchNumber}"
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetEntryStock(int id)
        {
            var entry = await _context.StockEntries
                .Include(s => s.StockMovements)
                .FirstOrDefaultAsync(s => s.EntryId == id);

            if (entry == null) return NotFound();

            var currentStock = entry.Quantity;
            return Json(new { currentStock });
        }
        //private async Task LoadViewBags()
        //{
        //    ViewBag.Products = await _context.Products
        //        .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.ProductName })
        //        .ToListAsync();

        //    ViewBag.StockEntries = await _context.StockEntries
        //        .Include(s => s.Warehouse)
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.EntryId.ToString(),
        //            Text = $"{s.Warehouse} - {s.BatchNumber}"
        //        })
        //        .ToListAsync();
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var movement = await _context.StockMovements
        //        .Include(s => s.Product)
        //        .Include(s => s.StockEntry)
        //        .Include(s => s.StockEntry.Warehouse)
        //        .FirstOrDefaultAsync(m => m.MovementId == id);

        //    if (movement == null) return NotFound();

        //    await LoadViewBags();
        //    return View(movement);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockMovement movement)
        {
            if (id != movement.MovementId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(movement);
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var originalMovement = await _context.StockMovements
                        .Include(m => m.StockEntry)
                        .FirstOrDefaultAsync(m => m.MovementId == id);

                    if (originalMovement == null)
                    {
                        return NotFound();
                    }

                    var originalStockEntry = originalMovement.StockEntry;

                    // Reverse original movement effect
                    if (originalMovement.MovementType == "IN")
                    {
                        originalStockEntry.Quantity -= originalMovement.Quantity;
                    }
                    else
                    {
                        originalStockEntry.Quantity += originalMovement.Quantity;
                    }

                    // Determine new StockEntry
                    StockEntry newStockEntry;
                    if (movement.StockEntryId != originalMovement.StockEntryId)
                    {
                        newStockEntry = await _context.StockEntries.FindAsync(movement.StockEntryId);
                        if (newStockEntry == null)
                        {
                            ModelState.AddModelError("StockEntryId", "المخزون المحدد غير موجود");
                            await LoadViewBags();
                            return View(movement);
                        }
                    }
                    else
                    {
                        newStockEntry = originalStockEntry;
                    }

                    // Apply new movement effect
                    if (movement.MovementType == "IN")
                    {
                        newStockEntry.Quantity += movement.Quantity;
                    }
                    else
                    {
                        if (movement.Quantity > newStockEntry.Quantity)
                        {
                            ModelState.AddModelError("Quantity", "الكمية المراد سحبها تتجاوز المخزون المتاح");
                            await LoadViewBags();
                            return View(movement);
                        }
                        newStockEntry.Quantity -= movement.Quantity;
                    }

                    // Update original movement properties
                    originalMovement.ProductId = movement.ProductId;
                    originalMovement.StockEntryId = movement.StockEntryId;
                    originalMovement.MovementType = movement.MovementType;
                    originalMovement.Quantity = movement.Quantity;
                    originalMovement.MovementDate = movement.MovementDate;
                    originalMovement.Reference = movement.Reference;
                    originalMovement.UpdatedAt = DateTime.UtcNow;
                    originalMovement.UpdatedBy = User.Identity?.Name ?? "System";

                    _context.Update(originalMovement);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "تم تحديث حركة المخزون بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating stock movement");
                    ModelState.AddModelError("", "حدث خطأ أثناء تحديث حركة المخزون");
                    await LoadViewBags();
                    return View(movement);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await MovementExists(movement.MovementId))
                {
                    return NotFound();
                }
                _logger.LogError(ex, "Concurrency error updating stock movement");
                ModelState.AddModelError("", "تم تعديل هذه الحركة بواسطة مستخدم آخر. يرجى تحديث الصفحة والمحاولة مرة أخرى.");
                await LoadViewBags();
                return View(movement);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var movement = await _context.StockMovements
                .Include(s => s.StockEntry)
                .FirstOrDefaultAsync(m => m.MovementId == id);

            if (movement == null) return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Reverse the movement's effect on stock
                var stockEntry = movement.StockEntry;
                stockEntry.Quantity -= movement.MovementType == "IN" ?
                    movement.Quantity : -movement.Quantity;

                _context.StockMovements.Remove(movement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "تم حذف حركة المخزون بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting stock movement");
                TempData["Error"] = "حدث خطأ أثناء حذف حركة المخزون";
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movement = await _context.StockMovements
                .Include(s => s.Product)
                .Include(s => s.StockEntry)
                .Include(s => s.StockEntry.Warehouse)
                .FirstOrDefaultAsync(m => m.MovementId == id);

            if (movement == null) return NotFound();

            return View(movement);
        }
        private async Task<bool> MovementExists(int id)
        {
            return await _context.StockMovements.AnyAsync(e => e.MovementId == id);
        }
    }
    }
