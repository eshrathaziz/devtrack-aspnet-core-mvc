using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager")]
public class DevelopersController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    public async Task<IActionResult> Index() => View(await context.DeveloperProfiles.Include(d => d.AssignedTasks).AsNoTracking().OrderBy(d => d.CurrentWorkload).ToListAsync());
    public IActionResult Create() => View(new DeveloperProfile());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeveloperProfile developer)
    {
        if (!ModelState.IsValid) return View(developer);
        context.DeveloperProfiles.Add(developer); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Developer Added", "Developer", developer.Id.ToString(), developer.DisplayName);
        TempData["Success"] = "Developer profile created."; return RedirectToAction(nameof(Details), new { id = developer.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var developer = await context.DeveloperProfiles.Include(d => d.AssignedTasks).ThenInclude(t => t.Project).Include(d => d.AssignedBugs).ThenInclude(b => b.Project).FirstOrDefaultAsync(d => d.Id == id);
        return developer is null ? NotFound() : View(developer);
    }
}
