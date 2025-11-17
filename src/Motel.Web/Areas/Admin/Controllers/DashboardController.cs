using Microsoft.AspNetCore.Mvc;
using Motel.Application.Interfaces;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly IReportsService _reportsService;

    public DashboardController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _reportsService.GetDashboardDataAsync();
        return View(data);
    }
}
