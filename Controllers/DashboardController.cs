using DevTrack.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Controllers;

public class DashboardController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index() => View(await dashboardService.BuildAsync());
}
