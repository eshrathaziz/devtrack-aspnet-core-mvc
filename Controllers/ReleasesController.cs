using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager,Developer,Tester")]
public class ReleasesController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    public async Task<IActionResult> Index(ReleaseStatus? status, int? projectId)
    {
        var query = context.Releases.Include(r => r.Project).Include(r => r.Bugs).AsNoTracking().AsQueryable();
        if (status.HasValue) query = query.Where(r => r.Status == status);
        if (projectId.HasValue) query = query.Where(r => r.ProjectId == projectId);
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", projectId);
        return View(await query.OrderByDescending(r => r.ReleaseDate).ToListAsync());
    }

    [Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create() { await PopulateProjectsAsync(); return View(new Release { ReleaseDate = DateTime.UtcNow.Date.AddDays(14) }); }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create(Release release)
    {
        if (await context.Releases.AnyAsync(r => r.ProjectId == release.ProjectId && r.Version == release.Version)) ModelState.AddModelError(nameof(release.Version), "This version already exists for the selected project.");
        if (!ModelState.IsValid) { await PopulateProjectsAsync(release.ProjectId); return View(release); }
        context.Releases.Add(release); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Release Created", "Release", release.Id.ToString(), release.Name);
        TempData["Success"] = "Release created."; return RedirectToAction(nameof(Details), new { id = release.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var release = await context.Releases.Include(r => r.Project).Include(r => r.Bugs).ThenInclude(b => b.AssignedDeveloper).FirstOrDefaultAsync(r => r.Id == id);
        return release is null ? NotFound() : View(release);
    }

    private async Task PopulateProjectsAsync(int? selected = null) => ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", selected);
}
