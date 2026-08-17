using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager,Developer,Tester")]
public class BugsController(ApplicationDbContext context, IActivityLogService activityLog, IWorkflowService workflow) : Controller
{
    public async Task<IActionResult> Index(BugStatus? status, BugSeverity? severity, int? projectId)
    {
        var query = context.Bugs.Include(b => b.Project).Include(b => b.AssignedDeveloper).AsNoTracking().AsQueryable();
        if (status.HasValue) query = query.Where(b => b.Status == status);
        if (severity.HasValue) query = query.Where(b => b.Severity == severity);
        if (projectId.HasValue) query = query.Where(b => b.ProjectId == projectId);
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", projectId);
        return View(await query.OrderByDescending(b => b.Severity).ThenByDescending(b => b.CreatedAtUtc).ToListAsync());
    }

    [Authorize(Roles = "Administrator,Project Manager,Tester")]
    public async Task<IActionResult> Create() { await PopulateLookupsAsync(); return View(new Bug()); }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager,Tester")]
    public async Task<IActionResult> Create(Bug bug)
    {
        bug.Reporter ??= User.Identity?.Name;
        if (!ModelState.IsValid) { await PopulateLookupsAsync(bug.ProjectId, bug.TaskItemId, bug.AssignedDeveloperId); return View(bug); }
        context.Bugs.Add(bug); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Bug Created", "Bug", bug.Id.ToString(), bug.Title);
        TempData["Success"] = "Bug recorded."; return RedirectToAction(nameof(Details), new { id = bug.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var bug = await context.Bugs.Include(b => b.Project).Include(b => b.TaskItem).Include(b => b.AssignedDeveloper).Include(b => b.Release).Include(b => b.Comments).FirstOrDefaultAsync(b => b.Id == id);
        return bug is null ? NotFound() : View(bug);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, BugStatus status)
    {
        var bug = await context.Bugs.FindAsync(id); if (bug is null) return NotFound();
        if (!workflow.CanTransition(bug.Status, status)) { TempData["Error"] = $"{bug.Status} cannot transition directly to {status}."; return RedirectToAction(nameof(Details), new { id }); }
        bug.Status = status; if (status == BugStatus.Closed) bug.ResolvedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Bug Status Updated", "Bug", id.ToString(), $"Changed {bug.Title} to {status}.");
        TempData["Success"] = "Bug status updated."; return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateLookupsAsync(int? project = null, int? task = null, int? developer = null)
    {
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", project);
        ViewBag.Tasks = new SelectList(await context.TaskItems.OrderBy(t => t.Title).ToListAsync(), "Id", "Title", task);
        ViewBag.Developers = new SelectList(await context.DeveloperProfiles.OrderBy(d => d.DisplayName).ToListAsync(), "Id", "DisplayName", developer);
    }
}
