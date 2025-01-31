using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class ProductVariantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductVariants
        public async Task<IActionResult> Index()
        {
            var variants = _context.ProductVariants.Include(v => v.Product);
            return View(await variants.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: ProductVariants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VariantId,ProductId,Size,Color,Style,PriceAdjustment")] ProductVariant variant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(variant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", variant.ProductId);
            return View(variant);
        }

        // GET: ProductVariants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", variant.ProductId);
            return View(variant);
        }

        // POST: ProductVariants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VariantId,ProductId,Size,Color,Style,PriceAdjustment")] ProductVariant variant)
        {
            if (id != variant.VariantId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductVariantExists(variant.VariantId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", variant.ProductId);
            return View(variant);
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
