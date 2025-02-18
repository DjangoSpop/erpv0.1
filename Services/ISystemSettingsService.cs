using erpv0._1.Data;
using erpv0._1.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace erpv0._1.Services
{
   
    public interface ISystemSettingsService
    {
        Task<SystemSettings?> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(SystemSettings settings);
        Task InitializeDefaultSettingsAsync(SystemSettings settings);
        IReadOnlyList<string> GetLanguageOptions();
        IReadOnlyList<string> GetTimeZoneOptions();
        IReadOnlyList<string> GetDateFormatOptions();
        IReadOnlyList<string> GetCurrencyOptions();
    }

    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemSettingsService> _logger;

        public SystemSettingsService(ApplicationDbContext context, ILogger<SystemSettingsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SystemSettings?> GetSettingsAsync()
        {
            try
            {
                return await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system settings");
                return null;
            }
        }

        public async Task<bool> UpdateSettingsAsync(SystemSettings settings)
        {
            if (settings == null)
            {
                _logger.LogWarning("Attempt to update settings with null value");
                return false;
            }

            try
            {
                var existingSettings = await _context.SystemSettings.FirstOrDefaultAsync();
                if (existingSettings == null)
                {
                    _logger.LogWarning("No existing settings found, initializing new settings.");
                    await _context.SystemSettings.AddAsync(settings);
                }
                else
                {
                    _context.Entry(existingSettings).CurrentValues.SetValues(settings);
                    _context.Entry(existingSettings).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Settings updated successfully.");
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating settings");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating settings");
                return false;
            }
        }

        public async Task InitializeDefaultSettingsAsync(SystemSettings settings)
        {
            if (settings == null)
            {
                _logger.LogWarning("Attempt to initialize with null default settings");
                return;
            }

            try
            {
                if (!await _context.SystemSettings.AnyAsync()) // Prevent duplication
                {
                    await _context.SystemSettings.AddAsync(settings);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Default settings initialized.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing default settings");
            }
        }

        public IReadOnlyList<string> GetLanguageOptions()
        {
            var languages = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Select(c => c.TwoLetterISOLanguageName)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            _logger.LogDebug("Fetched Language Options: {Languages}", string.Join(", ", languages));
            return languages;
        }

        public IReadOnlyList<string> GetTimeZoneOptions()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => tz.Id)
                .ToList();

            _logger.LogDebug("Fetched TimeZone Options: {TimeZones}", string.Join(", ", timeZones));
            return timeZones;
        }

        public IReadOnlyList<string> GetDateFormatOptions()
        {
            var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" };
            _logger.LogDebug("Fetched Date Format Options: {DateFormats}", string.Join(", ", dateFormats));
            return dateFormats;
        }

        public IReadOnlyList<string> GetCurrencyOptions()
        {
            var currencies = new List<string> { "SAR", "USD", "EUR", "EGP", "GBP" };
            _logger.LogDebug("Fetched Currency Options: {Currencies}", string.Join(", ", currencies));
            return currencies;
        }
    }
}
