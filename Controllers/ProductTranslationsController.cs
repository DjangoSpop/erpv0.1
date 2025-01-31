using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class ProductTranslationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductTranslationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductTranslations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductTranslations.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductTranslations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTranslation = await _context.ProductTranslations
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productTranslation == null)
            {
                return NotFound();
            }

            return View(productTranslation);
        }

        // GET: ProductTranslations/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: ProductTranslations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Language,Name,Description,Specifications,CreatedAt,UpdatedAt,CreatedBy,UpdatedBy")] ProductTranslation productTranslation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productTranslation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productTranslation.ProductId);
            return View(productTranslation);
        }

        // GET: ProductTranslations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTranslation = await _context.ProductTranslations.FindAsync(id);
            if (productTranslation == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productTranslation.ProductId);
            return View(productTranslation);
        }

        // POST: ProductTranslations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Language,Name,Description,Specifications,CreatedAt,UpdatedAt,CreatedBy,UpdatedBy")] ProductTranslation productTranslation)
        {
            if (id != productTranslation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productTranslation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTranslationExists(productTranslation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productTranslation.ProductId);
            return View(productTranslation);
        }

        // GET: ProductTranslations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTranslation = await _context.ProductTranslations
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productTranslation == null)
            {
                return NotFound();
            }

            return View(productTranslation);
        }

        // POST: ProductTranslations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productTranslation = await _context.ProductTranslations.FindAsync(id);
            if (productTranslation != null)
            {
                _context.ProductTranslations.Remove(productTranslation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTranslationExists(int id)
        {
            return _context.ProductTranslations.Any(e => e.Id == id);
        }
    }
}
