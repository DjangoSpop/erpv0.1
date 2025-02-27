using Microsoft.AspNetCore.Mvc;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using System.ComponentModel.DataAnnotations;

namespace erpv0._1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToListAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["Error"] = "حدث خطأ أثناء تحميل المنتجات";
                return View(new List<Product>());
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var product = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                    return NotFound();

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details for ID {ProductId}", id);
                TempData["Error"] = "حدث خطأ أثناء تحميل تفاصيل المنتج";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            LoadViewBagData();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? productImage)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBagData();
                return View(product);
            }

            try
            {
                //if (productImage != null)
                //{
                //    using var ms = new MemoryStream();
                //    await productImage.CopyToAsync(ms);
                //    product.ImageData = ms.ToArray();
                //    product.ImageContentType = productImage.ContentType;
                //}

                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة المنتج بنجاح";
                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", product.ProductName);
                ModelState.AddModelError("", "حدث خطأ أثناء إضافة المنتج");
                LoadViewBagData();
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound();

                LoadViewBagData();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for edit with ID {ProductId}", id);
                TempData["Error"] = "حدث خطأ أثناء تحميل المنتج للتعديل";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? productImage)
        {
            if (id != product.ProductId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadViewBagData();
                return View(product);
            }

            try
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                    return NotFound();

                existingProduct.ProductName = product.ProductName;
                existingProduct.BrandId = product.BrandId;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ListPrice = product.ListPrice;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                //if (productImage != null)
                //{
                //    using var ms = new MemoryStream();
                //    await productImage.CopyToAsync(ms);
                //    existingProduct.ImageData = ms.ToArray();
                //    existingProduct.ImageContentType = productImage.ContentType;
                //}

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تحديث المنتج بنجاح";
                return RedirectToAction(nameof(Details), new { id = existingProduct.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                ModelState.AddModelError("", "حدث خطأ أثناء تحديث المنتج");
                LoadViewBagData();
                return View(product);
            }
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound();

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم حذف المنتج بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                TempData["Error"] = "حدث خطأ أثناء حذف المنتج";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/GetProductImage/5
        //[HttpGet]
        //public async Task<IActionResult> GetProductImage(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null || product.ImageData == null)
        //        return NotFound();

        //    return File(product.ImageData, product.ImageContentType ?? "image/jpeg");
        //}

        private void LoadViewBagData()
        {
            ViewBag.Brands = new SelectList(_context.Brands.OrderBy(b => b.BrandName), "BrandId", "BrandName");
            ViewBag.Categories = new SelectList(_context.ProductCategories.OrderBy(c => c.Name), "CategoryId", "Name");
        }
    }
}
