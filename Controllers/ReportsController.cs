using DevTrack.Data;
using DevTrack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager")]
public class ReportsController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new ReportsViewModel
        {
            Projects = await context.Projects.Include(p => p.Client).OrderByDescending(p => p.Priority).ToListAsync(),
            Activities = await context.ActivityLogs.OrderByDescending(a => a.CreatedAtUtc).Take(20).ToListAsync(),
            TaskStatusCounts = await context.TaskItems.GroupBy(t => t.Status).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count()),
            BugSeverityCounts = await context.Bugs.GroupBy(b => b.Severity).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count())
        };
        return View(model);
    }
}
