using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Models;
using erpv0._1.Data;


namespace erpv0._1.Controllers
{
    public class StaffsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff
        public async Task<IActionResult> Index()
        {
            var staffList = await _context.Staffs
                .Include(s => s.Store)
                .Include(s => s.Manager)
                .ToListAsync();

            ViewBag.TotalStaff = staffList.Count;
            ViewBag.ActiveStaff = staffList.Count(s => s.Active == 1);
            ViewBag.InactiveStaff = staffList.Count(s => s.Active == 0);

            return View(staffList);
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var staff = await _context.Staffs
                .Include(s => s.Store)
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(m => m.StaffId == id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }

        // GET: Staff/Create
        public IActionResult Create()
        {
            ViewBag.Stores = _context.Stores.ToList();
            ViewBag.Managers = _context.Staffs.Where(s => s.ManagerId == null).ToList();
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: Staff/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
                return NotFound();

            ViewBag.Stores = _context.Stores.ToList();
            ViewBag.Managers = _context.Staffs.Where(s => s.ManagerId == null).ToList();
            return View(staff);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.StaffId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Staffs.Any(e => e.StaffId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var staff = await _context.Staffs
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.StaffId == id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
