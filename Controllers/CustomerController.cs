using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;



namespace erpv0._1.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _context.Customers.Select(c => new CustomerArabicViewModel
                {
                    CustomerId = c.CustomerId,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = $"{c.Street}, {c.City}, {c.State}, {c.ZipCode}"
                }).ToListAsync();

                return View(customers);
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework)
                TempData["Error"] = "حدث خطأ أثناء جلب قائمة العملاء";
                return View(new List<CustomerArabicViewModel>());
            }
        }

        // GET: Customers/Details/5
        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
                return NotFound();

            // Convert to ViewModel
            var viewModel = new CustomerArabicViewModel
            {
                CustomerId = customer.CustomerId,
                FullName = $"{customer.FirstName} {customer.LastName}",
                Phone = customer.Phone,
                Email = customer.Email,
                Address = $"{customer.Street}, {customer.City}, {customer.State}, {customer.ZipCode}"
            };

            return View(viewModel); // Now returning the correct ViewModel
        }


        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم إضافة العميل بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "حدث خطأ أثناء إضافة العميل";
                    return View(customer);
                }
            }
            return View(customer);
        }

        // GET: Customers/Edit/5[HttpPost]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث بيانات العميل بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e => e.CustomerId == customer.CustomerId))
                    {
                        TempData["Error"] = "العميل غير موجود";
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "حدث خطأ أثناء تحديث بيانات العميل";
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "حدث خطأ أثناء تحديث بيانات العميل";
                    return View(customer);
                }
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer != null)
                {
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم حذف العميل بنجاح";
                }
                else
                {
                    TempData["Error"] = "العميل غير موجود";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء حذف العميل. قد يكون مرتبطاً بطلبات أخرى";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
