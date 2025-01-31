using erpv0._1.Data;
using erpv0._1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Controllers
{
    public class StockReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockReportController> _logger;

        public StockReportController(ApplicationDbContext context, ILogger<StockReportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new StockReportViewModel
                {
                    Statistics = await GetStockStatistics(),
                    RecentMovements = await GetRecentMovements(),
                    Products = await GetProductDetails(),
                    TrendData = await GetStockTrends(),
                    CategoryDistribution = await GetCategoryDistribution()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating stock report");
                return View("Error");
            }
        }

        private async Task<StockStatistics> GetStockStatistics()
        {
            var stats = new StockStatistics
            {
                LastUpdated = DateTime.Now,
                TotalProducts = await _context.Products.CountAsync(),
                ActiveProducts = await _context.Stocks.Select(s => s.ProductId).Distinct().CountAsync(),
                TotalInventoryValue = await _context.StockEntries
                    .SumAsync(se => se.Quantity * se.CostPrice),

                LowStockItems = await _context.Stocks
                    .CountAsync(s => s.Quantity <= 10 && s.Quantity > 0),

                OutOfStockItems = await _context.Stocks
                    .CountAsync(s => s.Quantity <= 0),

                TotalMovements = await _context.StockMovements.CountAsync(),

                PendingTransfers = await _context.WarehouseTransfers
                    .CountAsync(wt => wt.Status == "Pending")
            };

            stats.AverageStockValue = (decimal)await _context.StockEntries
                .AverageAsync(se => (double)(se.Quantity * se.CostPrice));

            return stats;
        }

        private async Task<List<StockMovementSummary>> GetRecentMovements()
        {
            return await _context.StockMovements
                .Include(sm => sm.Product)
                .OrderByDescending(sm => sm.MovementDate)
                .Take(50)
                .Select(sm => new StockMovementSummary
                {
                    MovementId = sm.MovementId,
                    Date = sm.MovementDate,
                    MovementType = sm.MovementType,
                    ProductId = sm.ProductId,
                    ProductName = sm.Product.ProductName,
                    ProductCategory = sm.Product.Category.Name,
                    Quantity = sm.Quantity,
                    UnitValue = sm.StockEntry.CostPrice,
                    TotalValue = sm.Quantity * sm.StockEntry.CostPrice,
                    ReferenceNumber = sm.Reference,
                    Status = sm.MovementType,
                    InitiatedBy = sm.CreatedBy
                })
                .ToListAsync();
        }

        private async Task<List<StockProductDetail>> GetProductDetails()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.StockEntries)
                .Include(p => p.StockMovements)
                .Select(p => new StockProductDetail
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductId.ToString(),
                    ProductName = p.ProductName,
                    Category = p.Category.Name,
                    Brand = p.Brand.BrandName,
                    CurrentStock = p.StockEntries.Sum(se => se.Quantity),
                    UnitCost = p.StockEntries
                        .OrderByDescending(se => se.ReceiptDate)
                        .FirstOrDefault().CostPrice,
                    UnitPrice = p.ListPrice,
                    TotalValue = p.StockEntries.Sum(se => se.Quantity * se.CostPrice),
                    LastReceived = p.StockEntries
                        .OrderByDescending(se => se.ReceiptDate)
                        .FirstOrDefault().ReceiptDate,
                    LastIssued = p.StockMovements
                        .OrderByDescending(sm => sm.MovementDate)
                        .FirstOrDefault().MovementDate
                })
                .ToListAsync();
        }

        private async Task<List<StockTrendData>> GetStockTrends()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            return await _context.StockMovements
                .Where(sm => sm.MovementDate >= thirtyDaysAgo)
                .GroupBy(sm => sm.MovementDate.Date)
                .Select(g => new StockTrendData
                {
                    Date = g.Key,
                    TotalStock = g.Sum(sm => sm.Quantity),
                    TotalValue = g.Sum(sm => sm.Quantity * sm.StockEntry.CostPrice),
                    IncomingQuantity = g.Where(sm => sm.MovementType == "IN").Sum(sm => sm.Quantity),
                    OutgoingQuantity = g.Where(sm => sm.MovementType == "OUT").Sum(sm => sm.Quantity),
                    AverageUnitPrice = g.Average(sm => sm.StockEntry.CostPrice)
                })
                .OrderBy(st => st.Date)
                .ToListAsync();
        }

        private async Task<List<CategoryDistribution>> GetCategoryDistribution()
        {
            return await _context.ProductCategories
                .Include(pc => pc.Products)
                .ThenInclude(p => p.StockEntries)
                .Select(c => new CategoryDistribution
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count,
                    TotalStock = c.Products.SelectMany(p => p.StockEntries).Sum(se => se.Quantity),
                    TotalValue = c.Products.SelectMany(p => p.StockEntries)
                        .Sum(se => se.Quantity * se.CostPrice),
                    PercentageOfTotal = 0 // Calculate after fetching all data
                })
                .ToListAsync();
        }
    }
}
