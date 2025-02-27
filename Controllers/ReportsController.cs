using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using erpv0._1.Data;
using erpv0._1.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace erpv0._1.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(ReportFilterViewModel filters)
        {
            try
            {
                var viewModel = new ReportsIndexViewModel
                {
                    Filters = filters ?? new ReportFilterViewModel()
                };

                // Load lookup data for filters
                 await LoadFilterLookups();

                // Generate summary data
                viewModel.Summary = await GenerateReportSummary();

                // Apply filters and get paginated data
                var (entries, totalItems) = await GetFilteredStockEntries(filters);
                viewModel.StockEntries = entries;
                viewModel.Filters.TotalItems = totalItems;

                // Load warehouse statistics
                viewModel.WarehouseStocks = await GetWarehouseStatistics();

                // Load chart data
                viewModel.ChartData = await GenerateChartData();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                TempData["Error"] = "An error occurred while generating the report.";
                return View(new ReportsIndexViewModel());
            }
        }

        private async Task LoadFilterLookups()
        {
            ViewBag.Products = await _context.Products
                .OrderBy(p => p.ProductName)
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true)
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            ViewBag.Categories = await _context.ProductCategories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        private async Task<ReportSummaryViewModel> GenerateReportSummary()
        {
            var summary = new ReportSummaryViewModel();

            // Get total inventory value
            summary.TotalInventoryValue = await _context.StockEntries
                .SumAsync(s => s.Quantity * s.CostPrice);

            // Get product counts
            summary.TotalProducts = await _context.Products.CountAsync();
            summary.LowStockItems = await _context.StockEntries
                .CountAsync(s => s.Quantity > 0 && s.Quantity <= 10);
            summary.OutOfStockItems = await _context.StockEntries
                .CountAsync(s => s.Quantity <= 0);

            // Get pending transfers
            summary.PendingTransfers = await _context.WarehouseTransfers
                .CountAsync(w => w.Status == "Pending");

            return summary;
        }

        private async Task<(IEnumerable<StockEntryReportViewModel> entries, int totalItems)>
            GetFilteredStockEntries(ReportFilterViewModel filters)
        {
            var query = _context.StockEntries
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filters?.SearchTerm))
            {
                var search = filters.SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.BatchNumber.ToLower().Contains(search) ||
                    s.Product.ProductName.ToLower().Contains(search) ||
                    s.Warehouse.Name.ToLower().Contains(search));
            }

            if (filters?.ProductId.HasValue == true)
            {
                query = query.Where(s => s.ProductId == filters.ProductId);
            }

            if (filters?.WarehouseId.HasValue == true)
            {
                query = query.Where(s => s.WarehouseId == filters.WarehouseId);
            }

            if (filters?.StartDate.HasValue == true)
            {
                query = query.Where(s => s.ReceiptDate >= filters.StartDate);
            }

            if (filters?.EndDate.HasValue == true)
            {
                query = query.Where(s => s.ReceiptDate <= filters.EndDate);
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();

            // Apply pagination
            var entries = await query
                .OrderByDescending(s => s.ReceiptDate)
                .Skip((filters?.CurrentPage - 1 ?? 0) * filters?.PageSize ?? 10)
                .Take(filters?.PageSize ?? 10)
                .Select(s => new StockEntryReportViewModel
                {
                    EntryId = s.EntryId,
                    BatchNumber = s.BatchNumber,
                    ProductName = s.Product.ProductName,
                    WarehouseName = s.Warehouse.Name,
                    Quantity = s.Quantity,
                    CostPrice = s.CostPrice,
                    SellingPrice = s.SellingPrice,
                    ReceiptDate = s.ReceiptDate,
                    SupplierInvoice = s.SupplierInvoice
                })
                .ToListAsync();

            return (entries, totalItems);
        }

        private async Task<IEnumerable<WarehouseStockReportViewModel>> GetWarehouseStatistics()
        {
            return await _context.Warehouses
                .Where(w => w.IsActive == true)
                .Select(w => new WarehouseStockReportViewModel
                {
                    WarehouseName = w.Name,
                    Location = w.Location,
                    TotalProducts = w.StockEntries.Count(),
                    TotalValue = w.StockEntries.Sum(s => s.Quantity * s.CostPrice),
                    LowStockItems = w.StockEntries.Count(s => s.Quantity > 0 && s.Quantity <= 10),
                    CapacityUtilization = 0.0 // Calculate based on your business logic
                })
                .ToListAsync();
        }

        private async Task<Dictionary<string, List<ChartDataPoint>>> GenerateChartData()
        {
            var chartData = new Dictionary<string, List<ChartDataPoint>>();

            // Stock value by warehouse
            chartData["warehouseValue"] = await _context.Warehouses
                .Where(w => w.IsActive == true)
                .Select(w => new ChartDataPoint
                {
                    Label = w.Name,
                    Value = w.StockEntries.Sum(s => s.Quantity * s.CostPrice),
                    Color = "#" + ((w.WarehouseId * 4789) % 0xFFFFFF).ToString("X6")
                })
                .ToListAsync();

            // Add more chart data as needed

            return chartData;
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(ReportFilterViewModel filters)
        {
            try
            {
                // Implement Excel export logic using EPPlus or similar library
                return File(new byte[] { }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"inventory-report-{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                TempData["Error"] = "Failed to generate Excel report.";
                return RedirectToAction(nameof(Index), filters);
            }
        }
    }
}