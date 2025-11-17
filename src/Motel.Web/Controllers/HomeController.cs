using Microsoft.AspNetCore.Mvc;

namespace Motel.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Redirect to Admin Dashboard
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}
