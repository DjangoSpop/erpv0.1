using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.EntityFrameworkCore;

namespace erpv0._1.Services
{
    public interface IStockService
    {
        Task<decimal> GetAvailableStock(int productId, int warehouseId);
        Task<bool> ValidateStockAvailability(int productId, int warehouseId, int quantity);
    }

    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockService> _logger;



        public async Task<(bool success, string message)> CreateStockEntryAsync(StockEntry entry)
        {
            try
            {
                if (await _context.StockEntries.AnyAsync(s => s.BatchNumber == entry.BatchNumber))
                {
                    return (false, "رقم الدفعة موجود بالفعل");
                }

                entry.CreatedAt = DateTime.UtcNow;
                entry.CreatedBy = entry.CreatedBy ?? "system";

                _context.StockEntries.Add(entry);
                await _context.SaveChangesAsync();
                return (true, "تم إضافة السجل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء سجل المخزون");
                return (false, "حدث خطأ أثناء الحفظ");
            }
        }

        public async Task<(bool success, string message)> DeleteStockEntryAsync(int id)
        {
            try
            {
                var entry = await _context.StockEntries.FindAsync(id);
                if (entry == null) return (false, "السجل غير موجود");

                _context.StockEntries.Remove(entry);
                await _context.SaveChangesAsync();
                return (true, "تم حذف السجل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف السجل");
                return (false, "لا يمكن حذف هذا السجل");
            }
        }
        public StockService(ApplicationDbContext context, ILogger<StockService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<decimal> GetAvailableStock(int productId, int warehouseId)
        {
            try
            {
                var stockEntry = await _context.StockEntries
                    .Where(s => s.ProductId == productId && s.WarehouseId == warehouseId)
                    .SumAsync(s => s.Quantity);

                return stockEntry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating available stock for Product {ProductId} in Warehouse {WarehouseId}",
                    productId, warehouseId);
                throw;
            }
        }

        public async Task<bool> ValidateStockAvailability(int productId, int warehouseId, int quantity)
        {
            var availableStock = await GetAvailableStock(productId, warehouseId);
            return availableStock >= quantity;
        }
    }
}
