using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

public class ProjectsController(ApplicationDbContext db, ActivityService activity) : Controller
{
    public async Task<IActionResult> Index(string? search, string? status)
    {
        var query = db.Projects.Include(x => x.Client).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search) || x.ProjectCode.Contains(search) || x.Client!.CompanyName.Contains(search));
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ProjectStatus>(status, out var parsed)) query = query.Where(x => x.Status == parsed);
        ViewBag.Search = search ?? "";
        ViewBag.Status = status ?? "All statuses";
        ViewBag.Clients = await db.Clients.OrderBy(x => x.CompanyName).ToListAsync();
        return View(await query.OrderByDescending(x => x.Progress).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await db.Projects.Include(x => x.Client).Include(x => x.Requirements).Include(x => x.Sprints).Include(x => x.Tasks).Include(x => x.Bugs).Include(x => x.Releases).FirstOrDefaultAsync(x => x.Id == id);
        return project is null ? NotFound() : View(project);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
        project.ProjectCode = $"PRJ-{DateTime.UtcNow:yyMMddHHmm}";
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        await activity.RecordAsync("Olivia Morgan", "Created", "Project", project.ProjectCode, $"Created {project.Name} for a new client engagement.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ProjectStatus status)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return NotFound();
        project.Status = status;
        await db.SaveChangesAsync();
        await activity.RecordAsync("Olivia Morgan", "Updated", "Project", project.ProjectCode, $"Moved {project.Name} to {status}.");
        return RedirectToAction(nameof(Index));
    }
}
