using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using erpv0._1.Models;
using erpv0._1.Services;

namespace erpv0._1.Controllers
{
    public class SystemSettingsController : Controller
    {
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly ILogger<SystemSettingsController> _logger;

        public SystemSettingsController(ISystemSettingsService systemSettingsService, ILogger<SystemSettingsController> logger)
        {
            _systemSettingsService = systemSettingsService;
            _logger = logger;
        }

        // Display System Settings
        public async Task<IActionResult> Index()
        {
            var settings = await _systemSettingsService.GetSettingsAsync();
            if (settings == null)
            {
                _logger.LogWarning("System settings not found. Redirecting to edit page.");
                return RedirectToAction("Edit"); // Redirect user to the edit page
            }

            return View(settings);
        }

        // Edit System Settings
        public async Task<IActionResult> Edit()
        {
            var settings = await _systemSettingsService.GetSettingsAsync();
            if (settings == null)
            {
                settings = new SystemSettings(); // Create a default object if missing
            }

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SystemSettings settings)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid system settings data submitted.");
                return View(settings);
            }

            bool updated = await _systemSettingsService.UpdateSettingsAsync(settings);
            if (!updated)
            {
                ModelState.AddModelError("", "Failed to update system settings. Please try again.");
                return View(settings);
            }

            _logger.LogInformation("System settings updated successfully.");
            return RedirectToAction("Index");
        }
    }
}
