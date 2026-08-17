using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Controllers;

public class TasksController(ApplicationDbContext db, ActivityService activity) : Controller
{
    public async Task<IActionResult> Index(string? search, string? status)
    {
        var query = db.Tasks.Include(x => x.Project).Include(x => x.Sprint).Include(x => x.Developer).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Title.Contains(search) || x.TaskCode.Contains(search) || x.Project!.Name.Contains(search));
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<WorkStatus>(status, out var parsed)) query = query.Where(x => x.Status == parsed);
        ViewBag.Search = search ?? "";
        ViewBag.Status = status ?? "All statuses";
        return View(await query.OrderBy(x => x.Status).ThenBy(x => x.DueDate).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, WorkStatus status)
    {
        var task = await db.Tasks.Include(x => x.Project).FirstOrDefaultAsync(x => x.Id == id);
        if (task is null) return NotFound();
        var allowed = new Dictionary<WorkStatus, WorkStatus[]> {
            [WorkStatus.Backlog] = new[] { WorkStatus.ToDo, WorkStatus.Blocked },
            [WorkStatus.ToDo] = new[] { WorkStatus.InProgress, WorkStatus.Blocked },
            [WorkStatus.InProgress] = new[] { WorkStatus.CodeReview, WorkStatus.Blocked },
            [WorkStatus.CodeReview] = new[] { WorkStatus.Testing, WorkStatus.InProgress },
            [WorkStatus.Testing] = new[] { WorkStatus.Done, WorkStatus.InProgress },
            [WorkStatus.Blocked] = new[] { WorkStatus.ToDo, WorkStatus.InProgress },
            [WorkStatus.Done] = Array.Empty<WorkStatus>()
        };
        if (task.Status != status && !allowed.GetValueOrDefault(task.Status, Array.Empty<WorkStatus>()).Contains(status))
        {
            TempData["Error"] = $"{task.Status} cannot transition directly to {status}.";
            return RedirectToAction(nameof(Index));
        }
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await activity.RecordAsync("Daniel Kim", "Updated", "Task", task.TaskCode, $"Moved {task.Title} to {status}.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int id, int developerId)
    {
        var task = await db.Tasks.Include(x => x.Developer).FirstOrDefaultAsync(x => x.Id == id);
        var developer = await db.Developers.FindAsync(developerId);
        if (task is null || developer is null) return NotFound();
        task.DeveloperProfileId = developerId;
        task.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await activity.RecordAsync("Olivia Morgan", "Assigned", "Task", task.TaskCode, $"Assigned {task.Title} to {developer.Name}.");
        return RedirectToAction(nameof(Index));
    }
}
