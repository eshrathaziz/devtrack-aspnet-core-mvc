using System.Security.Claims;
using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize]
public class ProjectsController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    public async Task<IActionResult> Index(string? search, ProjectStatus? status, int? clientId)
    {
        var query = AuthorizedProjects().Include(p => p.Client).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(p => p.Name.Contains(search) || p.Client!.CompanyName.Contains(search));
        if (status.HasValue) query = query.Where(p => p.Status == status);
        if (clientId.HasValue) query = query.Where(p => p.ClientId == clientId);
        ViewBag.Clients = new SelectList(await context.Clients.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", clientId);
        return View(await query.OrderByDescending(p => p.Priority).ThenBy(p => p.EndDate).ToListAsync());
    }

    [Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create()
    {
        await PopulateClientsAsync(); return View(new Project { StartDate = DateTime.UtcNow.Date });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create(Project project)
    {
        if (project.EndDate.HasValue && project.EndDate < project.StartDate) ModelState.AddModelError(nameof(project.EndDate), "The end date cannot be before the start date.");
        if (!ModelState.IsValid) { await PopulateClientsAsync(project.ClientId); return View(project); }
        context.Projects.Add(project); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Project Created", "Project", project.Id.ToString(), $"Created project {project.Name}.");
        TempData["Success"] = "Project created."; return RedirectToAction(nameof(Details), new { id = project.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await AuthorizedProjects().Include(p => p.Client).Include(p => p.Requirements).Include(p => p.Sprints).ThenInclude(s => s.Tasks).Include(p => p.Bugs).Include(p => p.Releases).FirstOrDefaultAsync(p => p.Id == id);
        return project is null ? NotFound() : View(project);
    }

    [Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await context.Projects.FindAsync(id); if (project is null) return NotFound(); await PopulateClientsAsync(project.ClientId); return View(project);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        if (id != project.Id) return BadRequest();
        if (project.EndDate.HasValue && project.EndDate < project.StartDate) ModelState.AddModelError(nameof(project.EndDate), "The end date cannot be before the start date.");
        if (!ModelState.IsValid) { await PopulateClientsAsync(project.ClientId); return View(project); }
        context.Update(project); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Project Updated", "Project", project.Id.ToString(), $"Updated project {project.Name}.");
        TempData["Success"] = "Project updated."; return RedirectToAction(nameof(Details), new { id });
    }

    private IQueryable<Project> AuthorizedProjects()
    {
        var query = context.Projects.AsQueryable();
        if (User.IsInRole("Client")) { var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); query = query.Where(p => p.Client!.ApplicationUserId == userId); }
        return query;
    }

    private async Task PopulateClientsAsync(int? selected = null) => ViewBag.Clients = new SelectList(await context.Clients.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", selected);
}
