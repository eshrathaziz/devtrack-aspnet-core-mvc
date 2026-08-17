using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager,Developer,Tester")]
public class SprintsController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    public async Task<IActionResult> Index(int? projectId, SprintStatus? status)
    {
        var query = context.Sprints.Include(s => s.Project).Include(s => s.Tasks).AsNoTracking().AsQueryable();
        if (projectId.HasValue) query = query.Where(s => s.ProjectId == projectId);
        if (status.HasValue) query = query.Where(s => s.Status == status);
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", projectId);
        return View(await query.OrderByDescending(s => s.StartDate).ToListAsync());
    }

    [Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create() { await PopulateProjectsAsync(); return View(new Sprint { StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(14) }); }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create(Sprint sprint)
    {
        if (sprint.EndDate <= sprint.StartDate) ModelState.AddModelError(nameof(sprint.EndDate), "A sprint end date must be after the start date.");
        if (!ModelState.IsValid) { await PopulateProjectsAsync(sprint.ProjectId); return View(sprint); }
        context.Sprints.Add(sprint); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Sprint Created", "Sprint", sprint.Id.ToString(), sprint.Name);
        TempData["Success"] = "Sprint created."; return RedirectToAction(nameof(Details), new { id = sprint.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var sprint = await context.Sprints.Include(s => s.Project).Include(s => s.Tasks).ThenInclude(t => t.Developer).FirstOrDefaultAsync(s => s.Id == id);
        return sprint is null ? NotFound() : View(sprint);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> UpdateStatus(int id, SprintStatus status)
    {
        var sprint = await context.Sprints.FindAsync(id); if (sprint is null) return NotFound();
        sprint.Status = status; await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Sprint Status Updated", "Sprint", id.ToString(), $"Changed {sprint.Name} to {status}.");
        TempData["Success"] = "Sprint status updated."; return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateProjectsAsync(int? selected = null) => ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", selected);
}
