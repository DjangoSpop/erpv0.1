using erpv0._1.Data;
using erpv0._1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace erpv0._1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = new HomeViewModel
                {
                    Stats = await GetDashboardStats(),
                    RecentActivities = await GetRecentActivities(),
                    InventoryMetrics = await GetInventoryMetrics(),
                    WarehouseUtilization = await GetWarehouseUtilization()
                };
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new HomeViewModel
                {
                    Stats = new DashboardStats(),
                    RecentActivities = new List<RecentActivityViewModel>(),
                    InventoryMetrics = new InventoryMetricsViewModel(),
                    WarehouseUtilization = new List<WarehouseUtilizationViewModel>()
                });
            }
        }

        private async Task<DashboardStats> GetDashboardStats()
        {
            return new DashboardStats
            {
                TotalProducts = await _context.Products.CountAsync(),
                ActiveWarehouses = await _context.Warehouses.CountAsync(w => w.IsActive == true),
                TotalInventoryValue = await _context.StockEntries
                    .SumAsync(se => se.Quantity * se.CostPrice),
                PendingTransfers = await _context.WarehouseTransfers
                    .CountAsync(wt => wt.Status == "Pending"),
                LowStockItems = await _context.Products
                    .CountAsync(p => _context.StockEntries
                        .Where(se => se.ProductId == p.ProductId)
                        .Sum(se => se.Quantity) < 10)
            };
        }

        private async Task<List<RecentActivityViewModel>> GetRecentActivities()
        {
            var activities = new List<RecentActivityViewModel>();

            // Get recent stock movements
            var recentMovements = await _context.StockMovements
                .Include(sm => sm.Product)
                .OrderByDescending(sm => sm.MovementDate)
                .Take(5)
                .Select(sm => new RecentActivityViewModel
                {
                    Date = sm.MovementDate,
                    ActivityType = "Stock Movement",
                    Description = $"{sm.Quantity} units of {sm.Product.ProductName}",
                    Status = sm.MovementType
                })
                .ToListAsync();

            activities.AddRange(recentMovements);

            // Get recent transfers
            var recentTransfers = await _context.WarehouseTransfers
                .Include(wt => wt.Product)
                .Include(wt => wt.SourceWarehouse)
                .Include(wt => wt.DestWarehouse)
                .OrderByDescending(wt => wt.RequestedDate)
                .Take(5)
                .Select(wt => new RecentActivityViewModel
                {
                    Date = wt.RequestedDate,
                    ActivityType = "Transfer",
                    Description = $"Transfer from {wt.SourceWarehouse.Name} to {wt.DestWarehouse.Name}",
                    Status = wt.Status
                })
                .ToListAsync();

            activities.AddRange(recentTransfers);

            return activities.OrderByDescending(a => a.Date).Take(5).ToList();
        }

        private async Task<InventoryMetricsViewModel> GetInventoryMetrics()
        {
            var totalStockEntries = await _context.StockEntries.CountAsync();
            var totalMovements = await _context.StockMovements.CountAsync();

            // Handle empty sequence for AverageStockValue
            var averageStockValue = totalStockEntries > 0
                ? await _context.StockEntries.AverageAsync(se => se.Quantity * se.CostPrice)
                : 0;

            var stockOutItems = await _context.Products
                .CountAsync(p => !_context.StockEntries
                    .Any(se => se.ProductId == p.ProductId && se.Quantity > 0));
            return new InventoryMetricsViewModel
            {
                TotalStockEntries = totalStockEntries,
                TotalMovements = totalMovements,
                AverageStockValue = averageStockValue,
                StockOutItems = stockOutItems
            };
        }

        private async Task<List<WarehouseUtilizationViewModel>> GetWarehouseUtilization()
        {
            return await _context.Warehouses
                .Where(w => w.IsActive == true)
                .Select(w => new WarehouseUtilizationViewModel
                {
                    WarehouseName = w.Name,
                    TotalCapacity = 1000, // This should come from warehouse settings
                    CurrentStock = _context.StockEntries
                        .Where(se => se.WarehouseId == w.WarehouseId)
                        .Sum(se => se.Quantity)
                })
                .ToListAsync();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}