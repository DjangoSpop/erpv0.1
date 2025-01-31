using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Services
{
    public interface IStockEntryService
    {
        Task<List<StockEntry>> GetAllStockEntriesAsync();
        Task<StockEntry?> GetStockEntryByIdAsync(int id);
        Task<(bool success, string message)> CreateStockEntryAsync(StockEntry entry);
        Task<(bool success, string message)> UpdateStockEntryAsync(StockEntry entry);
        Task<(bool success, string message)> DeleteStockEntryAsync(int id);
        Task<List<Warehouse>> GetActiveWarehousesAsync();
        Task<List<Product>> GetProductsAsync();
        Task<bool> ValidateStockEntryAsync(StockEntry entry);
    }

    public class StockEntryService : IStockEntryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockEntryService> _logger;

        public StockEntryService(ApplicationDbContext context, ILogger<StockEntryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<StockEntry>> GetAllStockEntriesAsync()
        {
            return await _context.StockEntries
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.Warehouse)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<StockEntry?> GetStockEntryByIdAsync(int id)
        {
            return await _context.StockEntries
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.EntryId == id);
        }

        public async Task<(bool success, string message)> CreateStockEntryAsync(StockEntry entry)
        {
            try
            {
                if (!await ValidateStockEntryAsync(entry))
                {
                    return (false, "البيانات غير صالحة");
                }

                entry.CreatedAt = DateTime.UtcNow;
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return (true, "تم إضافة السجل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء سجل المخزون");
                return (false, "حدث خطأ أثناء حفظ البيانات");
            }
        }

        public async Task<(bool success, string message)> UpdateStockEntryAsync(StockEntry entry)
        {
            try
            {
                if (!await ValidateStockEntryAsync(entry))
                {
                    return (false, "البيانات غير صالحة");
                }

                var existingEntry = await _context.StockEntries.FindAsync(entry.EntryId);
                if (existingEntry == null)
                {
                    return (false, "السجل غير موجود");
                }

                entry.UpdatedAt = DateTime.UtcNow;
                entry.CreatedAt = existingEntry.CreatedAt;
                entry.CreatedBy = existingEntry.CreatedBy;

                _context.Entry(existingEntry).CurrentValues.SetValues(entry);
                await _context.SaveChangesAsync();
                return (true, "تم تحديث السجل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث سجل المخزون");
                return (false, "حدث خطأ أثناء تحديث البيانات");
            }
        }

        public async Task<(bool success, string message)> DeleteStockEntryAsync(int id)
        {
            try
            {
                var entry = await _context.StockEntries
                    .Include(s => s.StockMovements)
                    .FirstOrDefaultAsync(s => s.EntryId == id);

                if (entry == null)
                {
                    return (false, "السجل غير موجود");
                }

                if (entry.StockMovements.Any())
                {
                    return (false, "لا يمكن حذف السجل لوجود حركات مخزون مرتبطة به");
                }

                _context.StockEntries.Remove(entry);
                await _context.SaveChangesAsync();
                return (true, "تم حذف السجل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف سجل المخزون");
                return (false, "حدث خطأ أثناء حذف السجل");
            }
        }

        public async Task<List<Warehouse>> GetActiveWarehousesAsync()
        {
            return await _context.Warehouses
                .Where(w => w.IsActive == true)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<bool> ValidateStockEntryAsync(StockEntry entry)
        {
            // التحقق من وجود المنتج
            var product = await _context.Products.FindAsync(entry.ProductId);
            if (product == null) return false;

            // التحقق من وجود المستودع
            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.WarehouseId == entry.WarehouseId && w.IsActive == true);
            if (warehouse == null) return false;

            // التحقق من عدم تكرار رقم الدفعة
            var duplicateBatch = await _context.StockEntries
                .AnyAsync(s => s.BatchNumber == entry.BatchNumber
                    && s.EntryId != entry.EntryId);
            if (duplicateBatch) return false;

            // التحقق من صحة الأسعار
            if (entry.SellingPrice < entry.CostPrice)
            {
                _logger.LogWarning("سعر البيع أقل من سعر التكلفة للسجل");
            }

            return true;
        }
    }
}