using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using erpv0._1.Models.ViewModels;

namespace erpv0._1.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Store)
                    .Include(o => o.Staff)
                    .Include(o => o.OrderItems)
                    .Select(o => new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer != null ? o.Customer.FirstName + " " + o.Customer.LastName : "غير معروف",
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate,
                        RequiredDate = o.RequiredDate,
                        ShippedDate = o.ShippedDate,
                        StoreId = o.StoreId,
                        StoreName = o.Store.StoreName,
                        StaffId = o.StaffId,
                        StaffName = o.Staff != null ? o.Staff.FirstName + " " + o.Staff.LastName : "غير معروف",
                        ItemCount = o.OrderItems.Count,
                        TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * (oi.ListPrice - oi.Discount))
                    })
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return View("Error");
            }
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Store)
                    .Include(o => o.Staff)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Where(o => o.OrderId == id)
                    .Select(o => new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer != null ? o.Customer.FirstName + " " + o.Customer.LastName : "غير معروف",
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate,
                        RequiredDate = o.RequiredDate,
                        ShippedDate = o.ShippedDate,
                        StoreId = o.StoreId,
                        StoreName = o.Store.StoreName,
                        StaffId = o.StaffId,
                        StaffName = o.Staff != null ? o.Staff.FirstName + " " + o.Staff.LastName : "غير معروف",
                        ItemCount = o.OrderItems.Count,
                        TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * (oi.ListPrice - oi.Discount))
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound();
                }

                // Get order items for display
                ViewBag.OrderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == id)
                    .Include(oi => oi.Product)
                    .Select(oi => new OrderItemViewModel
                    {
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        ListPrice = oi.ListPrice,
                        Discount = oi.Discount,
                    })
                    .ToListAsync();

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id}");
                return View("Error");
            }
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            try
            {
                PopulateDropDownLists();

                return View(new OrderViewModel
                {
                    OrderDate = DateOnly.FromDateTime(DateTime.Today),
                    RequiredDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                    OrderStatus = 1 // Pending
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing order creation page");
                return View("Error");
            }
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var order = new Order
                    {
                        CustomerId = model.CustomerId,
                        OrderStatus = model.OrderStatus,
                        OrderDate = model.OrderDate,
                        RequiredDate = model.RequiredDate,
                        ShippedDate = model.ShippedDate,
                        StoreId = model.StoreId,
                        StaffId = model.StaffId
                    };

                    _context.Add(order);
                    await _context.SaveChangesAsync();

                    // Redirect to add order items
                    return RedirectToAction("ByOrder", "OrderItem", new { orderId = order.OrderId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating order");
                    ModelState.AddModelError("", "حدث خطأ أثناء إنشاء الطلب. الرجاء المحاولة مرة أخرى.");
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var order = await _context.Orders
                    .Where(o => o.OrderId == id)
                    .Select(o => new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate,
                        RequiredDate = o.RequiredDate,
                        ShippedDate = o.ShippedDate,
                        StoreId = o.StoreId,
                        StaffId = o.StaffId
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound();
                }

                PopulateDropDownLists(order);
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id} for edit");
                return View("Error");
            }
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel model)
        {
            if (id != model.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = await _context.Orders.FindAsync(id);
                    if (order == null)
                    {
                        return NotFound();
                    }

                    order.CustomerId = model.CustomerId;
                    order.OrderStatus = model.OrderStatus;
                    order.RequiredDate = model.RequiredDate;
                    order.ShippedDate = model.ShippedDate;
                    order.StoreId = model.StoreId;
                    order.StaffId = model.StaffId;

                    _context.Update(order);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!OrderExists(model.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, $"Concurrency error updating order with ID {id}");
                        ModelState.AddModelError("", "حدث خطأ أثناء تحديث الطلب. ربما تم تعديله بواسطة مستخدم آخر.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating order with ID {id}");
                    ModelState.AddModelError("", "حدث خطأ أثناء تحديث الطلب. الرجاء المحاولة مرة أخرى.");
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Store)
                    .Include(o => o.Staff)
                    .Include(o => o.OrderItems)
                    .Where(o => o.OrderId == id)
                    .Select(o => new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer != null ? o.Customer.FirstName + " " + o.Customer.LastName : "غير معروف",
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate,
                        RequiredDate = o.RequiredDate,
                        ShippedDate = o.ShippedDate,
                        StoreId = o.StoreId,
                        StoreName = o.Store.StoreName,
                        StaffId = o.StaffId,
                        StaffName = o.Staff != null ? o.Staff.FirstName + " " + o.Staff.LastName : "غير معروف",
                        ItemCount = o.OrderItems.Count,
                        TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * (oi.ListPrice - oi.Discount))
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id} for delete");
                return View("Error");
            }
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order != null)
                {
                    // First remove all order items
                    _context.OrderItems.RemoveRange(order.OrderItems);

                    // Then remove the order
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID {id}");
                return View("Error");
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        private void PopulateDropDownLists(OrderViewModel model = null)
        {
            // Customers dropdown
            ViewBag.Customers = new SelectList(_context.Customers
                .Select(c => new
                {
                    c.CustomerId,
                    FullName = c.FirstName + " " + c.LastName
                })
                .OrderBy(c => c.FullName),
                "CustomerId", "FullName", model?.CustomerId);

            // Stores dropdown
            ViewBag.Stores = new SelectList(_context.Stores, "StoreId", "StoreName", model?.StoreId);

            // Staff dropdown
            ViewBag.Staff = new SelectList(_context.Staffs
                .Select(s => new
                {
                    s.StaffId,
                    FullName = s.FirstName + " " + s.LastName
                })
                .OrderBy(s => s.FullName),
                "StaffId", "FullName", model?.StaffId);

            // Order statuses dropdown
            ViewBag.OrderStatuses = new SelectList(OrderViewModel.GetOrderStatuses(), "Key", "Value", model?.OrderStatus);
        }
    }
}