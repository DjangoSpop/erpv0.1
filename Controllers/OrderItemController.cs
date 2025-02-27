using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Models.ViewModels;

namespace erpv0._1.Controllers
{
    public class OrderItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderItemController> _logger;

        public OrderItemController(ApplicationDbContext context, ILogger<OrderItemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: OrderItem
        public async Task<IActionResult> Index()
        {
            try
            {
                var orderItems = await _context.OrderItems
                    .Include(o => o.Order)
                    .Include(o => o.Product)
                    .Select(oi => new OrderItemViewModel
                    {
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName, // Assuming Product has a ProductName property
                        Quantity = oi.Quantity,
                        ListPrice = oi.ListPrice,
                        Discount = oi.Discount,
                        OrderNumber = oi.Order.OrderId // Using OrderId as OrderNumber for display
                    })
                    .ToListAsync();

                return View(orderItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order items");
                return View("Error");
            }
        }

        // GET: OrderItem/Details/5/10
        public async Task<IActionResult> Details(int? orderId, int? itemId)
        {
            if (orderId == null || itemId == null)
            {
                return NotFound();
            }

            try
            {
                var orderItem = await _context.OrderItems
                    .Include(o => o.Order)
                    .Include(o => o.Product)
                    .Where(oi => oi.OrderId == orderId && oi.ItemId == itemId)
                    .Select(oi => new OrderItemViewModel
                    {
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName, // Assuming Product has a ProductName property
                        Quantity = oi.Quantity,
                        ListPrice = oi.ListPrice,
                        Discount = oi.Discount,
                        OrderNumber = oi.Order.OrderId // Using OrderId as OrderNumber for display
                    })
                    .FirstOrDefaultAsync();

                if (orderItem == null)
                {
                    return NotFound();
                }

                return View(orderItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order item with OrderId {orderId} and ItemId {itemId}");
                return View("Error");
            }
        }

        // GET: OrderItem/Create
        public IActionResult Create()
        {
            try
            {
                // Populate ViewBags for dropdowns
                ViewBag.Orders = new SelectList(_context.Orders, "OrderId", "OrderId");
                ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName"); // Assuming Product has a ProductName property

                // Get the next available ItemId
                var nextItemId = 1;
                if (_context.OrderItems.Any())
                {
                    nextItemId = _context.OrderItems.Max(oi => oi.ItemId) + 1;
                }

                var model = new OrderItemViewModel
                {
                    ItemId = nextItemId,
                    Quantity = 1,
                    Discount = 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create order item form");
                return View("Error");
            }
        }

        // POST: OrderItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the combination of OrderId and ItemId already exists
                    var exists = await _context.OrderItems.AnyAsync(oi =>
                        oi.OrderId == model.OrderId && oi.ItemId == model.ItemId);

                    if (exists)
                    {
                        ModelState.AddModelError("", "عنصر الطلب هذا موجود بالفعل");
                        ViewBag.Orders = new SelectList(_context.Orders, "OrderId", "OrderId", model.OrderId);
                        ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName", model.ProductId);
                        return View(model);
                    }

                    // Get the product to check the price
                    var product = await _context.Products.FindAsync(model.ProductId);
                    if (product == null)
                    {
                        ModelState.AddModelError("ProductId", "المنتج المحدد غير موجود");
                        ViewBag.Orders = new SelectList(_context.Orders, "OrderId", "OrderId", model.OrderId);
                        ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName", model.ProductId);
                        return View(model);
                    }

                    var orderItem = new OrderItem
                    {
                        OrderId = model.OrderId,
                        ItemId = model.ItemId,
                        ProductId = model.ProductId,
                        Quantity = model.Quantity,
                        ListPrice = model.ListPrice,
                        Discount = model.Discount
                    };

                    _context.Add(orderItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating order item");
                    ModelState.AddModelError("", "حدث خطأ أثناء إنشاء عنصر الطلب. الرجاء المحاولة مرة أخرى.");
                }
            }

            ViewBag.Orders = new SelectList(_context.Orders, "OrderId", "OrderId", model.OrderId);
            ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName", model.ProductId);
            return View(model);
        }

        // GET: OrderItem/Edit/5/10
        public async Task<IActionResult> Edit(int? orderId, int? itemId)
        {
            if (orderId == null || itemId == null)
            {
                return NotFound();
            }

            try
            {
                var orderItem = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderId && oi.ItemId == itemId)
                    .Select(oi => new OrderItemViewModel
                    {
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        ListPrice = oi.ListPrice,
                        Discount = oi.Discount
                    })
                    .FirstOrDefaultAsync();

                if (orderItem == null)
                {
                    return NotFound();
                }

                ViewBag.Orders = new SelectList(_context.Orders, "OrderId", "OrderId", orderItem.OrderId);
                ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName", orderItem.ProductId);

                return View(orderItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order item with OrderId {orderId} and ItemId {itemId} for edit");
                return View("Error");
            }
        }

        // POST: OrderItem/Edit/5/10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int orderId, int itemId, OrderItemViewModel model)
        {
            if (orderId != model.OrderId || itemId != model.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orderItem = await _context.OrderItems
                        .Where(oi => oi.OrderId == orderId && oi.ItemId == itemId)
                        .FirstOrDefaultAsync();

                    if (orderItem == null)
                    {
                        return NotFound();
                    }

                    // Update the properties
                    orderItem.ProductId = model.ProductId;
                    orderItem.Quantity = model.Quantity;
                    orderItem.ListPrice = model.ListPrice;
                    orderItem.Discount = model.Discount;

                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!OrderItemExists(model.OrderId, model.ItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, $"Concurrency error updating order item with OrderId {orderId} and ItemId {itemId}");
                        ModelState.AddModelError("", "حدث خطأ أثناء تحديث عنصر الطلب. ربما تم تعديله بواسطة مستخدم آخر.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating order item with OrderId {orderId} and ItemId {itemId}");
                    ModelState.AddModelError("", "حدث خطأ أثناء تحديث عنصر الطلب. الرجاء المحاولة مرة أخرى.");
                }
            }

            ViewBag.Orders = new SelectList(_context.Orders, "OrderId", "OrderId", model.OrderId);
            ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName", model.ProductId);
            return View(model);
        }

        // GET: OrderItem/Delete/5/10
        public async Task<IActionResult> Delete(int? orderId, int? itemId)
        {
            if (orderId == null || itemId == null)
            {
                return NotFound();
            }

            try
            {
                var orderItem = await _context.OrderItems
                    .Include(o => o.Order)
                    .Include(o => o.Product)
                    .Where(oi => oi.OrderId == orderId && oi.ItemId == itemId)
                    .Select(oi => new OrderItemViewModel
                    {
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        ListPrice = oi.ListPrice,
                        Discount = oi.Discount,
                        OrderNumber = oi.Order.OrderId
                    })
                    .FirstOrDefaultAsync();

                if (orderItem == null)
                {
                    return NotFound();
                }

                return View(orderItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order item with OrderId {orderId} and ItemId {itemId} for delete");
                return View("Error");
            }
        }

        // POST: OrderItem/Delete/5/10
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int orderId, int itemId)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderId && oi.ItemId == itemId)
                    .FirstOrDefaultAsync();

                if (orderItem != null)
                {
                    _context.OrderItems.Remove(orderItem);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order item with OrderId {orderId} and ItemId {itemId}");
                return View("Error");
            }
        }

        private bool OrderItemExists(int orderId, int itemId)
        {
            return _context.OrderItems.Any(e => e.OrderId == orderId && e.ItemId == itemId);
        }

        // GET: OrderItem/ByOrder/5
        public async Task<IActionResult> ByOrder(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            try
            {
                var orderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .Include(o => o.Order)
                    .Include(o => o.Product)
                    .Select(oi => new OrderItemViewModel
                    {
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        ListPrice = oi.ListPrice,
                        Discount = oi.Discount,
                        OrderNumber = oi.Order.OrderId
                    })
                    .ToListAsync();

                // Get order details for the view title
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return NotFound();
                }

                ViewBag.OrderId = orderId;
                ViewBag.OrderDate = order.OrderDate;

                return View(orderItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order items for OrderId {orderId}");
                return View("Error");
            }
        }
    }
}
