using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class ProductVariantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductVariant> _logger;
        public ProductVariantsController(ApplicationDbContext context, ILogger<ProductVariant> logger)
        {

            _context = context;
            _logger = logger;
        }
        //public async Task<IActionResult> Index(string? searchTerm, String? isActive, int page = 1)
        //{
        //    try
        //    {
        //        var query = _context.Warehouses
        //            .Include(w => w.StockEntries)
        //            .AsQueryable();

        //        // Apply filters
        //        if (!string.IsNullOrEmpty(searchTerm))
        //        {
        //            query = query.Where(w =>
        //                w.Code.Contains(searchTerm) ||
        //                w.Name.Contains(searchTerm) ||
        //                w.Location.Contains(searchTerm));
        //        }
        //        bool? isActiveFilter = null;
        //        if (!string.IsNullOrEmpty(isActive))
        //        {
        //            if (bool.TryParse(isActive, out bool parsedValue))
        //            {
        //                isActiveFilter = parsedValue;
        //                query = query.Where(w => w.IsActive == parsedValue);
        //            }
        //        }
        //        // Calculate statistics
        //        var statistics = new WarehouseStatistics
        //        {
        //            TotalWarehouses = await query.CountAsync(),
        //            ActiveWarehouses = await query.CountAsync(w => w.IsActive == true),
        //            TotalStockValue = await _context.StockEntries.SumAsync(se => se.Quantity * se.CostPrice),
        //            LowStockProducts = await _context.StockEntries.CountAsync(se => se.Quantity <= 10 && se.Quantity > 0),
        //            OutOfStockProducts = await _context.StockEntries.CountAsync(se => se.Quantity <= 0)
        //        };

        //        // Setup pagination
        //        var pageSize = 10;
        //        var skip = (page - 1) * pageSize;

        //        // Get warehouses
        //        var warehouses = await query
        //            .OrderByDescending(w => w.UpdatedAt)
        //            .Skip(skip)
        //            .Take(pageSize)
        //            .Select(w => w.ToViewModel())
        //            .ToListAsync();

        //        var viewModel = new WarehouseListViewModel
        //        {
        //            Warehouses = warehouses,
        //            SearchTerm = searchTerm,
        //            IsActiveFilter = isActiveFilter,
        //            Statistics = statistics,
        //            Pagination = new PaginationInfo
        //            {
        //                CurrentPage = page,
        //                ItemsPerPage = pageSize,
        //                TotalItems = statistics.TotalWarehouses
        //            }
        //        };

        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error loading warehouses list");
        //        TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
        //        return View(new WarehouseListViewModel());
        //    }
        //}
        // GET: ProductVariants


            public async Task<IActionResult> Index(string? searchTerm, int? productId, string? size, string? color, int page = 1)
            {
                try
                {
                    var query = _context.ProductVariants
                        .Include(pv => pv.Product)
                        .AsQueryable();

                    // Apply filters
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query = query.Where(pv =>
                            pv.Size.Contains(searchTerm) ||
                            pv.Color.Contains(searchTerm) ||
                            pv.Style.Contains(searchTerm) ||
                            pv.Product.ProductName.Contains(searchTerm));
                    }

                    if (productId.HasValue)
                    {
                        query = query.Where(pv => pv.ProductId == productId);
                    }

                    if (!string.IsNullOrEmpty(size))
                    {
                        query = query.Where(pv => pv.Size.Contains(size));
                    }

                    if (!string.IsNullOrEmpty(color))
                    {
                        query = query.Where(pv => pv.Color.Contains(color));
                    }

                    // Setup pagination
                    var pageSize = 10;
                    var skip = (page - 1) * pageSize;

                    // Get product variants
                    var productVariants = await query
                        .OrderByDescending(pv => pv.UpdatedAt)
                        .Skip(skip)
                        .Take(pageSize)
                        .ToListAsync();

                    // Get total count for pagination
                    var totalItems = await query.CountAsync();

                    // Prepare dropdown lists
                    var products = await _context.Products
                        .Select(p => new SelectListItem
                        {
                            Value = p.ProductId.ToString(),
                            Text = p.ProductName
                        })
                        .ToListAsync();

                    var sizes = await _context.ProductVariants
                        .Select(pv => pv.Size)
                        .Distinct()
                        .Select(s => new SelectListItem
                        {
                            Value = s,
                            Text = s
                        })
                        .ToListAsync();

                    var colors = await _context.ProductVariants
                        .Select(pv => pv.Color)
                        .Distinct()
                        .Select(c => new SelectListItem
                        {
                            Value = c,
                            Text = c
                        })
                        .ToListAsync();

                    var viewModel = new ProductVariantIndexViewModel
                    {
                        ProductVariants = productVariants,
                        SearchTerm = searchTerm,
                        ProductId = productId,
                        Size = size,
                        Color = color,
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                        Products = products,
                        Sizes = sizes,
                        Colors = colors
                    };

                    return View(viewModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading product variants list");
                    TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
                    return View(new ProductVariantIndexViewModel());
                }
            }
        
    


    // GET: ProductVariants/Details/5
    public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null) return NotFound();

            return View(variant);
        }

        // GET: ProductVariants/Create
        // GET: ProductVariants/Create
        public async Task<IActionResult> Create()
        {
            var products = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
                .ToListAsync();

            var viewModel = new ProductVariantViewModel
            {
                Products = products
            };

            return View(viewModel);
        }


        // POST: ProductVariants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariantViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var variant = new ProductVariant
                {
                    ProductId = viewModel.ProductId,
                    Size = viewModel.Size,
                    Color = viewModel.Color,
                    Style = viewModel.Style,
                    PriceAdjustment = viewModel.PriceAdjustment,
                    IsActive = viewModel.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System" // Replace with actual user when auth is implemented
                };
            
                //var warehouse = model.ToEntity();
                //_context.Add(warehouse);
                //await _context.SaveChangesAsync();

                //TempData["Success"] = "تم إضافة المستودع بنجاح";
                //return RedirectToAction(nameof(Index));
                
                _context.Add(variant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed; reload the products for the dropdown
            viewModel.Products = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
                .ToListAsync();

            return View(viewModel);
        }

        // POST: ProductVariants/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null) return NotFound();

            var viewModel = new ProductVariantViewModel
            {
                VariantId = variant.VariantId,
                ProductId = variant.ProductId,
                Size = variant.Size,
                Color = variant.Color,
                Style = variant.Style,
                PriceAdjustment = variant.PriceAdjustment,
                IsActive = variant.IsActive == false,
                Products = await _context.Products
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = p.ProductName
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVariantViewModel viewModel)
        {
            if (id != viewModel.VariantId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var variant = await _context.ProductVariants.FindAsync(id);
                    if (variant == null) return NotFound();

                    variant.ProductId = viewModel.ProductId;
                    variant.Size = viewModel.Size;
                    variant.Color = viewModel.Color;
                    variant.Style = viewModel.Style;
                    variant.PriceAdjustment = viewModel.PriceAdjustment;
                    variant.IsActive = viewModel.IsActive;

                    _context.Update(variant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductVariantExists(viewModel.VariantId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed; reload the products for the dropdown
            viewModel.Products = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
                .ToListAsync();

            return View(viewModel);
        }


        // GET: ProductVariants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null) return NotFound();

            return View(variant);
        }

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant != null)
            {
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantExists(int id)
        {
            return _context.ProductVariants.Any(e => e.VariantId == id);
        }
        [HttpGet]
        public async Task<IActionResult> CheckVariantDependencies(int variantId)
        {
            try
            {
                // Check if there are any stock entries for this variant
                var hasStockEntries = await _context.StockEntries
                    .AnyAsync(se => se.ProductVariantVariantId == variantId);

                // Check if there are any order items with this variant
                var hasOrderItems = await _context.OrderItems
                    .AnyAsync(oi => oi.ProductId == variantId);

                // You can add more dependency checks as needed
                var dependencies = new
                {
                    HasStockEntries = hasStockEntries,
                    HasOrderItems = hasOrderItems,
                    HasDependencies = hasStockEntries || hasOrderItems
                };

                return Json(dependencies);
                //genrate a catch block
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }
    }
}
