using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

public class RequirementsController(ApplicationDbContext db, ActivityService activity) : Controller
{
    public async Task<IActionResult> Index(string? search, string? status)
    {
        var query = db.Requirements.Include(x => x.Project).Include(x => x.Client).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Title.Contains(search) || x.RequirementCode.Contains(search) || x.Client!.CompanyName.Contains(search));
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RequirementStatus>(status, out var parsed)) query = query.Where(x => x.Status == parsed);
        ViewBag.Search = search ?? "";
        ViewBag.Status = status ?? "All statuses";
        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, RequirementStatus status)
    {
        var requirement = await db.Requirements.FindAsync(id);
        if (requirement is null) return NotFound();
        requirement.Status = status;
        await db.SaveChangesAsync();
        await activity.RecordAsync("Olivia Morgan", status == RequirementStatus.Approved ? "Approved" : "Updated", "Requirement", requirement.RequirementCode, $"Moved {requirement.Title} to {status}.");
        return RedirectToAction(nameof(Index));
    }
}
