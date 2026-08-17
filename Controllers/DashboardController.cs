using System.Security.Claims;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Controllers;

[Authorize]
public class DashboardController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var clientOnly = User.IsInRole("Client");
        return View(await dashboardService.GetDashboardAsync(userId, clientOnly));
    }
}
