using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace erpv0._1.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BrandsController> _logger;

        public BrandsController(ApplicationDbContext context, ILogger<BrandsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            try
            {
                var brands = await _context.Brands.ToListAsync();
                var brandViewModels = brands.Select(b => new BrandViewModel { BrandId = b.BrandId, BrandName = b.BrandName }).ToList();
                return View(brandViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving brands");
                TempData["Error"] = "حدث خطأ أثناء جلب قائمة العلامات التجارية";
                return View(new List<BrandViewModel>());
            }
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            var viewModel = new BrandViewModel { BrandId = brand.BrandId, BrandName = brand.BrandName };
            return View(viewModel);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandViewModel brandViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var brand = new Brand { BrandName = brandViewModel.BrandName };
                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم إضافة العلامة التجارية بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating brand");
                    TempData["Error"] = "حدث خطأ أثناء إضافة العلامة التجارية";
                }
            }
            return View(brandViewModel);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            var viewModel = new BrandViewModel { BrandId = brand.BrandId, BrandName = brand.BrandName };
            return View(viewModel);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandViewModel brandViewModel)
        {
            if (id != brandViewModel.BrandId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var brand = await _context.Brands.FindAsync(id);
                    if (brand == null)
                    {
                        TempData["Error"] = "العلامة التجارية غير موجودة";
                        return NotFound();
                    }
                    brand.BrandName = brandViewModel.BrandName;
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث العلامة التجارية بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brandViewModel.BrandId))
                    {
                        TempData["Error"] = "العلامة التجارية غير موجودة";
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Concurrency error updating brand");
                        TempData["Error"] = "حدث خطأ أثناء تحديث العلامة التجارية";
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating brand with ID {id}");
                    TempData["Error"] = "حدث خطأ أثناء تحديث العلامة التجارية";
                }
            }
            return View(brandViewModel);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            var viewModel = new BrandViewModel { BrandId = brand.BrandId, BrandName = brand.BrandName };
            return View(viewModel);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand != null)
                {
                    _context.Brands.Remove(brand);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم حذف العلامة التجارية بنجاح";
                }
                else
                {
                    TempData["Error"] = "العلامة التجارية غير موجودة";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting brand with ID {id}");
                TempData["Error"] = "حدث خطأ أثناء حذف العلامة التجارية. قد تكون مرتبطة بمنتجات أخرى";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.BrandId == id);
        }
    }
}