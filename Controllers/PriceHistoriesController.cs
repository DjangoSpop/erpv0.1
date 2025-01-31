using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class PriceHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriceHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PriceHistories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PriceHistories.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PriceHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceHistory = await _context.PriceHistories
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceHistory == null)
            {
                return NotFound();
            }

            return View(priceHistory);
        }

        // GET: PriceHistories/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: PriceHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,CostPrice,SellingPrice,EffectiveDate,Reason,CreatedAt,UpdatedAt,CreatedBy,UpdatedBy")] PriceHistory priceHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", priceHistory.ProductId);
            return View(priceHistory);
        }

        // GET: PriceHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceHistory = await _context.PriceHistories.FindAsync(id);
            if (priceHistory == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", priceHistory.ProductId);
            return View(priceHistory);
        }

        // POST: PriceHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,CostPrice,SellingPrice,EffectiveDate,Reason,CreatedAt,UpdatedAt,CreatedBy,UpdatedBy")] PriceHistory priceHistory)
        {
            if (id != priceHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceHistoryExists(priceHistory.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", priceHistory.ProductId);
            return View(priceHistory);
        }

        // GET: PriceHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceHistory = await _context.PriceHistories
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceHistory == null)
            {
                return NotFound();
            }

            return View(priceHistory);
        }

        // POST: PriceHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceHistory = await _context.PriceHistories.FindAsync(id);
            if (priceHistory != null)
            {
                _context.PriceHistories.Remove(priceHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceHistoryExists(int id)
        {
            return _context.PriceHistories.Any(e => e.Id == id);
        }
    }
}
