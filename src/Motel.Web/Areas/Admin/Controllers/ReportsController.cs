using Microsoft.AspNetCore.Mvc;
using Motel.Application.Interfaces;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ReportsController : Controller
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    public async Task<IActionResult> Occupancy(string? date)
    {
        var reportDate = string.IsNullOrEmpty(date)
            ? DateOnly.FromDateTime(DateTime.Today)
            : DateOnly.Parse(date);

        var data = await _reportsService.GetDailyOccupancyAsync(reportDate);
        ViewBag.Date = reportDate.ToString("yyyy-MM-dd");
        return View(data);
    }

    public async Task<IActionResult> Revenue(int? year)
    {
        var reportYear = year ?? DateTime.Now.Year;
        var data = await _reportsService.GetMonthlyRevenueAsync(reportYear);
        ViewBag.Year = reportYear;
        return View(data);
    }
}
