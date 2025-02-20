using AutoMapper;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(
            ApplicationDbContext context,
            ILogger<SuppliersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            try
            {
                var suppliers = await _context.Suppliers
                    .Include(s => s.Invoices)
                    .Select(s => new SupplierViewModel
                    {
                        SupplierId = s.SupplierId,
                        Name = s.Name,
                        ContactName = s.ContactName,
                        ContactEmail = s.ContactEmail,
                        ContactPhone = s.ContactPhone,
                        City = s.City,
                        Address = s.Address,
                        InvoiceCount = s.Invoices.Count
                    })
                    .ToListAsync();

                return View(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving suppliers");
                return View("Error");
            }
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null)
            {
                return NotFound();
            }

            var viewModel = new SupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                ContactName = supplier.ContactName,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                City = supplier.City,
                Address = supplier.Address,
                InvoiceCount = supplier.Invoices.Count
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(m => m.SupplierId == id);

            if (supplier == null)
            {
                return NotFound();
            }

            var viewModel = new SupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                ContactName = supplier.ContactName,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                City = supplier.City,
                Address = supplier.Address,
                InvoiceCount = supplier.Invoices.Count
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierViewModel viewModel)
        {
            if (id != viewModel.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var supplier = await _context.Suppliers.FindAsync(id);
                    if (supplier == null)
                    {
                        return NotFound();
                    }

                    // Manual mapping
                    supplier.Name = viewModel.Name;
                    supplier.ContactName = viewModel.ContactName;
                    supplier.ContactEmail = viewModel.ContactEmail;
                    supplier.ContactPhone = viewModel.ContactPhone;
                    supplier.City = viewModel.City;
                    supplier.Address = viewModel.Address;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث بيانات المورد بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(viewModel.SupplierId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "حدث خطأ أثناء حفظ التغييرات. الرجاء المحاولة مرة أخرى.");
                        throw;
                    }
                }
            }
            return View(viewModel);
        }


        public IActionResult Create()
        {
            var viewModel = new SupplierViewModel
            {
                // Set any default values if needed
            };
            return View(viewModel);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var supplier = new Supplier
                    {
                        Name = viewModel.Name,
                        ContactName = viewModel.ContactName,
                        ContactEmail = viewModel.ContactEmail,
                        ContactPhone = viewModel.ContactPhone,
                        City = viewModel.City,
                        Address = viewModel.Address
                    };

                    _context.Add(supplier);
                    await _context.SaveChangesAsync();

                    // You might want to add a success message here
                    TempData["Success"] = "تم إضافة المورد بنجاح";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating supplier");
                    ModelState.AddModelError("", "حدث خطأ أثناء إضافة المورد");
                }
            }
            return View(viewModel);
        }
        // POST: Suppliers/Edit/5


        //// GET: Suppliers/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var supplier = await _context.Suppliers
        //        .Include(s => s.Invoices)
        //        .FirstOrDefaultAsync(m => m.SupplierId == id);

        //    if (supplier == null)
        //    {
        //        return NotFound();
        //    }

        //    var viewModel = _mapper.Map<SupplierViewModel>(supplier);
        //    viewModel.InvoiceCount = supplier.Invoices.Count;

        //    return View(viewModel);
        //}

        // POST: Suppliers/Delete/5


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var supplier = await _context.Suppliers
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }
            var viewModel = new SupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                ContactName = supplier.ContactName,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                City = supplier.City,
                Address = supplier.Address,
                InvoiceCount = supplier.Invoices.Count
            };
            return View(viewModel);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
