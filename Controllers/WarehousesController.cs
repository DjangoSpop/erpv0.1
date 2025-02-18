using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace erpv0._1.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WarehousesController> _logger;

        public WarehousesController(
            ApplicationDbContext context,
            ILogger<WarehousesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, String? isActive, int page = 1)
        {
            try
            {
                var query = _context.Warehouses
                    .Include(w => w.StockEntries)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(w =>
                        w.Code.Contains(searchTerm) ||
                        w.Name.Contains(searchTerm) ||
                        w.Location.Contains(searchTerm));
                }
                bool? isActiveFilter = null;
                if (!string.IsNullOrEmpty(isActive))
                {
                    if (bool.TryParse(isActive, out bool parsedValue))
                    {
                        isActiveFilter = parsedValue;
                        query = query.Where(w => w.IsActive == parsedValue);
                    }
                }
                // Calculate statistics
                var statistics = new WarehouseStatistics
                {
                    TotalWarehouses = await query.CountAsync(),
                    ActiveWarehouses = await query.CountAsync(w => w.IsActive == true),
                    TotalStockValue = await _context.StockEntries.SumAsync(se => se.Quantity * se.CostPrice),
                    LowStockProducts = await _context.StockEntries.CountAsync(se => se.Quantity <= 10 && se.Quantity > 0),
                    OutOfStockProducts = await _context.StockEntries.CountAsync(se => se.Quantity <= 0)
                };

                // Setup pagination
                var pageSize = 10;
                var skip = (page - 1) * pageSize;

                // Get warehouses
                var warehouses = await query
                    .OrderByDescending(w => w.UpdatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(w => w.ToViewModel())
                    .ToListAsync();

                var viewModel = new WarehouseListViewModel
                {
                    Warehouses = warehouses,
                    SearchTerm = searchTerm,
                    IsActiveFilter = isActiveFilter,
                    Statistics = statistics,
                    Pagination = new PaginationInfo 
                    {
                        CurrentPage = page,
                        ItemsPerPage = pageSize,
                        TotalItems = statistics.TotalWarehouses
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading warehouses list");
                TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
                return View(new WarehouseListViewModel());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new WarehouseViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var warehouse = model.ToEntity();
                _context.Add(warehouse);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة المستودع بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse");
                ModelState.AddModelError("", "حدث خطأ أثناء إضافة المستودع");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseViewModel model)
        {
            if (id != model.WarehouseId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    return NotFound();
                }

                model.ToEntity(warehouse);
                _context.Update(warehouse);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تحديث المستودع بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(model.WarehouseId))
                {
                    return NotFound();
                }
                throw;
            }
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(c => c.WarehouseId == id);
            if (warehouse == null) return NotFound();

            return View(warehouse);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    return NotFound();
                }

                warehouse.IsActive = !(warehouse.IsActive ?? false);
                warehouse.UpdatedAt = DateTime.UtcNow;
                warehouse.UpdatedBy = "System"; // Replace with actual user when auth is implemented

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling warehouse status");
                return Json(new { success = false, message = "حدث خطأ أثناء تحديث الحالة" });
            }
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.WarehouseId == id);
        }
    }
}