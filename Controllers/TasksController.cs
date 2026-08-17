using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager,Developer,Tester")]
public class TasksController(ApplicationDbContext context, IActivityLogService activityLog, IWorkflowService workflow) : Controller
{
    public async Task<IActionResult> Index(string? search, TaskStatus? status, Priority? priority, int? projectId, int? developerId)
    {
        var query = context.TaskItems.Include(t => t.Project).Include(t => t.Sprint).Include(t => t.Developer).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(t => t.Title.Contains(search) || t.Project!.Name.Contains(search));
        if (status.HasValue) query = query.Where(t => t.Status == status);
        if (priority.HasValue) query = query.Where(t => t.Priority == priority);
        if (projectId.HasValue) query = query.Where(t => t.ProjectId == projectId);
        if (developerId.HasValue) query = query.Where(t => t.DeveloperProfileId == developerId);
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", projectId);
        ViewBag.Developers = new SelectList(await context.DeveloperProfiles.OrderBy(d => d.DisplayName).ToListAsync(), "Id", "DisplayName", developerId);
        return View(await query.OrderBy(t => t.DueDate).ThenByDescending(t => t.Priority).ToListAsync());
    }

    [Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create() { await PopulateLookupsAsync(); return View(new TaskItem()); }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Create(TaskItem task)
    {
        if (!ModelState.IsValid) { await PopulateLookupsAsync(task.ProjectId, task.SprintId, task.RequirementId, task.DeveloperProfileId); return View(task); }
        context.TaskItems.Add(task); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Task Created", "Task", task.Id.ToString(), task.Title);
        TempData["Success"] = "Task created."; return RedirectToAction(nameof(Details), new { id = task.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var task = await context.TaskItems.Include(t => t.Project).Include(t => t.Sprint).Include(t => t.Requirement).Include(t => t.Developer).Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == id);
        ViewBag.Developers = new SelectList(await context.DeveloperProfiles.OrderBy(d => d.DisplayName).ToListAsync(), "Id", "DisplayName", task?.DeveloperProfileId);
        return task is null ? NotFound() : View(task);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, TaskStatus status)
    {
        var task = await context.TaskItems.FindAsync(id); if (task is null) return NotFound();
        if (!workflow.CanTransition(task.Status, status)) { TempData["Error"] = $"{task.Status} cannot transition directly to {status}."; return RedirectToAction(nameof(Details), new { id }); }
        task.Status = status; await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Task Status Updated", "Task", id.ToString(), $"Changed {task.Title} to {status}.");
        TempData["Success"] = "Task status updated."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Assign(int id, int? developerProfileId)
    {
        var task = await context.TaskItems.FindAsync(id); if (task is null) return NotFound();
        task.DeveloperProfileId = developerProfileId; await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Task Assigned", "Task", id.ToString(), task.Title);
        TempData["Success"] = "Task assignment updated."; return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateLookupsAsync(int? project = null, int? sprint = null, int? requirement = null, int? developer = null)
    {
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", project);
        ViewBag.Sprints = new SelectList(await context.Sprints.OrderByDescending(s => s.StartDate).ToListAsync(), "Id", "Name", sprint);
        ViewBag.Requirements = new SelectList(await context.Requirements.OrderBy(r => r.Title).ToListAsync(), "Id", "Title", requirement);
        ViewBag.Developers = new SelectList(await context.DeveloperProfiles.OrderBy(d => d.DisplayName).ToListAsync(), "Id", "DisplayName", developer);
    }
}
