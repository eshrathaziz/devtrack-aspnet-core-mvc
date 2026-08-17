using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

public class BugsController(ApplicationDbContext db, ActivityService activity) : Controller
{
    public async Task<IActionResult> Index(string? search, string? status)
    {
        var query = db.Bugs.Include(x => x.Project).Include(x => x.Developer).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Title.Contains(search) || x.BugCode.Contains(search) || x.Project!.Name.Contains(search));
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BugStatus>(status, out var parsed)) query = query.Where(x => x.Status == parsed);
        ViewBag.Search = search ?? "";
        ViewBag.Status = status ?? "All statuses";
        return View(await query.OrderByDescending(x => x.Severity).ThenByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, BugStatus status)
    {
        var bug = await db.Bugs.FindAsync(id);
        if (bug is null) return NotFound();
        bug.Status = status;
        bug.ResolvedAt = status == BugStatus.Closed || status == BugStatus.Fixed ? DateTime.UtcNow : null;
        await db.SaveChangesAsync();
        await activity.RecordAsync("Sofia Alvarez", status == BugStatus.Closed ? "Resolved" : "Updated", "Bug", bug.BugCode, $"Moved {bug.Title} to {status}.");
        return RedirectToAction(nameof(Index));
    }
}
