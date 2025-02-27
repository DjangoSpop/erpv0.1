using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stocks
        public async Task<IActionResult> Index()
        {
            var stocks = await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Store)
                .ToListAsync();
            return View(stocks);
        }

        // GET: Stocks/Details?storeId=1&productId=2
        public async Task<IActionResult> Details(int? storeId, int? productId)
        {
            if (storeId == null || productId == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Store)
                .FirstOrDefaultAsync(s => s.StoreId == storeId && s.ProductId == productId);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductName");
            ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreName");
            return View();
        }

        // POST: Stocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StoreId,ProductId,Quantity")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductName", stock.ProductId);
            ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreName", stock.StoreId);
            return View(stock);
        }

        // GET: Stocks/Edit?storeId=1&productId=2
        public async Task<IActionResult> Edit(int? storeId, int? productId)
        {
            if (storeId == null || productId == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.StoreId == storeId && s.ProductId == productId);
            if (stock == null)
            {
                return NotFound();
            }
            ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductName", stock.ProductId);
            ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreName", stock.StoreId);
            return View(stock);
        }

        // POST: Stocks/Edit?storeId=1&productId=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int storeId, int productId, [Bind("StoreId,ProductId,Quantity")] Stock stock)
        {
            // Confirm both keys match the record being updated.
            if (storeId != stock.StoreId || productId != stock.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.StoreId, stock.ProductId))
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
            ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductName", stock.ProductId);
            ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreName", stock.StoreId);
            return View(stock);
        }

        // GET: Stocks/Delete?storeId=1&productId=2
        public async Task<IActionResult> Delete(int? storeId, int? productId)
        {
            if (storeId == null || productId == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Store)
                .FirstOrDefaultAsync(s => s.StoreId == storeId && s.ProductId == productId);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Delete?storeId=1&productId=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int storeId, int productId)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.StoreId == storeId && s.ProductId == productId);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int storeId, int productId)
        {
            return _context.Stocks.Any(s => s.StoreId == storeId && s.ProductId == productId);
        }
    }
}
