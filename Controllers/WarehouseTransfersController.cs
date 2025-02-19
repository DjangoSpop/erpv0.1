using AspNetCoreGeneratedDocument;
using erpv0._1.Data;
using erpv0._1.Models;
using erpv0._1.Models.ViewModels.WarhousesTransfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace erpv0._1.Controllers
{
    public class WarehouseTransfersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WarehouseTransfersController> _logger;


        public WarehouseTransfersController(
            ApplicationDbContext context,
            ILogger<WarehouseTransfersController> logger
           )
        {
            _context = context;
            _logger = logger;

        }

        public async Task<IActionResult> Index(
      string searchTerm,
      string status,
      DateTime? startDate,
      DateTime? endDate,
      int? sourceWarehouseId,
      int? destWarehouseId,
      int pageNumber = 1)
        {
            ViewBag.Warehouses = new SelectList(_context.Warehouses.ToList(), "WarehouseId", "Name");
            ViewBag.Products = new SelectList(_context.Products.ToList(), "ProductId", "ProductName");
            try
            {
                var query = _context.WarehouseTransfers
                    .Include(w => w.DestWarehouse)
                    .Include(w => w.SourceWarehouse)
                    .Include(w => w.Product)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(w =>
                        w.Product.ProductName.Contains(searchTerm) ||
                        w.SourceWarehouse.Name.Contains(searchTerm) ||
                        w.DestWarehouse.Name.Contains(searchTerm));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(w => w.Status == status);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(w => w.RequestedDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(w => w.RequestedDate <= endDate.Value);
                }

                if (sourceWarehouseId.HasValue)
                {
                    query = query.Where(w => w.SourceWarehouseId == sourceWarehouseId.Value);
                }

                if (destWarehouseId.HasValue)
                {
                    query = query.Where(w => w.DestWarehouseId == destWarehouseId.Value);
                }

                const int pageSize = 10;
                var transfers = await query
                    .OrderByDescending(w => w.RequestedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalRecords = await query.CountAsync();

                var viewModel = new Models.ViewModels.WarhousesTransfer.WarehouseTransferListViewModel
                {
                    Products = await _context.Products
            .OrderBy(p => p.ProductName)
            .ToListAsync(),
                    Transfers = transfers,
                    PagingInfo = new Models.ViewModels.WarhousesTransfer.PagingInfo
                    {
                        CurrentPage = pageNumber,
                        ItemsPerPage = pageSize,
                        TotalItems = totalRecords
                    },
                    Warehouses = await _context.Warehouses
                        .Where(w => w.IsActive == true)
                        .OrderBy(w => w.Name)
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse transfers");
                TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
                return View(new Models.ViewModels.WarhousesTransfer.WarehouseTransferListViewModel());
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsByWarehouse(int warehouseId)
        {
            var products = await _context.Products
       //.Where(p => p.WarehouseId == warehouseId) // if applicable
       .Select(p => new { productId = p.ProductId, productName = p.ProductName })
       .ToListAsync();

            return Json(products);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var transfer = await _context.WarehouseTransfers.FindAsync(id);
            if (transfer == null)
            {
                return Json(new { success = false, message = "طلب النقل غير موجود." });
            }

            transfer.Status = status;
            _context.Update(transfer);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تم تحديث حالة طلب النقل بنجاح." });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new WarehouseTransferCreateViewModel();
                await PrepareCreateViewData(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing create view");
                TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseTransferCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareCreateViewData(model);
                return View(model);
            }

            if (model.SourceWarehouseId == model.DestWarehouseId)
            {
                ModelState.AddModelError("", "لا يمكن أن يكون المستودع المصدر والوجهة نفس المستودع");
                await PrepareCreateViewData(model);
                return View(model);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var sourceStock = await _context.StockEntries
                        .Where(s => s.WarehouseId == model.SourceWarehouseId && s.ProductId == model.ProductId)
                        .SumAsync(s => (int?)s.Quantity) ?? 0;

                    if (model.Quantity > sourceStock)
                    {
                        ModelState.AddModelError("", "الكمية المطلوبة غير متوفرة في المستودع المصدر");
                        await PrepareCreateViewData(model);
                        return View(model);
                    }

                    var transfer = new WarehouseTransfer
                    {
                        SourceWarehouseId = model.SourceWarehouseId,
                        DestWarehouseId = model.DestWarehouseId,
                        ProductId = model.ProductId,
                        Quantity = model.Quantity,
                        Status = "Pending",
                        RequestedDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = User?.Identity?.Name ?? "System",
                        UpdatedBy = User?.Identity?.Name ?? "System",
                        ApprovedBy = string.Empty
                    };

                    _context.Add(transfer);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "تم إنشاء طلب النقل بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating warehouse transfer");
                    ModelState.AddModelError("", "حدث خطأ أثناء إنشاء طلب النقل");
                    await PrepareCreateViewData(model);
                    return View(model);
                }
            }
        }

        public IActionResult Details(int id)
        {
            var transfer = _context.WarehouseTransfers.Find(id);
            if (transfer == null)
            {
                return NotFound();
            }
            return View(transfer);
        }

        public async Task PrepareCreateViewData(WarehouseTransferCreateViewModel model)
        {
            // Populate AvailableWarehouses from your database
            model.AvailableWarehouses = await _context.Warehouses
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            // Optionally, if you want to preload available products:
            model.AvailableProducts = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
                .ToListAsync();
        }
        private async Task PrepareCreateViewData()
        {
           
            try
            {

                ViewBag.Warehouses = await _context.Warehouses
                    .OrderBy(w => w.Name)
                    .Select(w => new SelectListItem
                    {
                        Value = w.WarehouseId.ToString(),
                        Text = w.Name
                    })
                    .ToListAsync();

                ViewBag.Products = await _context.Products
                    .OrderBy(p => p.ProductName)
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = p.ProductName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dropdown data for warehouses and products.");
                ViewBag.Warehouses = new List<SelectListItem>();
                ViewBag.Products = new List<SelectListItem>();
                TempData["Error"] = "حدث خطأ أثناء تحميل بيانات المخازن والمنتجات.";
            }
        }
    }

}

        //[HttpPost, ActionName("Create")]
        //public async Task<IActionResult> Create(WarehouseTransferCreateViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return Json(new { success = false, message = "بيانات غير صحيحة" });

        //    try
        //    {
        //        var transfer = new WarehouseTransfer
        //        {
        //            SourceWarehouseId = model.SourceWarehouseId,
        //            DestWarehouseId = model.DestWarehouseId,
        //            ProductId = model.ProductId,
        //            Quantity = model.Quantity,
        //            Status = "Pending",
        //            RequestedDate = DateTime.Now,
        //            CreatedAt = DateTime.Now,
        //            CreatedBy = User?.Identity?.Name ?? "System",
        //            UpdatedBy = User?.Identity?.Name ?? "System"
        //        };

        //        _context.WarehouseTransfers.Add(transfer);
        //        await _context.SaveChangesAsync();

        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating transfer");
        //        return Json(new { success = false, message = "حدث خطأ أثناء إنشاء الطلب" });
        //    }
        //}
        //public async Task<IActionResult> Create()
        //{
        //    try
        //    {
        //        await PrepareCreateViewData();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error preparing create view");
        //        TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        //// POST: WarehouseTransfers/Create
     
        //private async Task PrepareCreateViewData()
        //{
        //    ViewData["Warehouses"] = await _context.Warehouses
        //        .Where(w => w.IsActive == true)
        //        .OrderBy(w => w.Name)
        //        .Select(w => new SelectListItem
        //        {
        //            Value = w.WarehouseId.ToString(),
        //            Text = $"{w.Name} ({w.Location})"
        //        })
        //        .ToListAsync();

//    ViewData["Products"] = await _context.Products
//        .OrderBy(p => p.ProductName)
//        .Select(p => new SelectListItem
//        {
//            Value = p.ProductId.ToString(),
//            Text = p.ProductName
//        })
//        .ToListAsync();
//}


