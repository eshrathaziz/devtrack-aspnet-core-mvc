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
public class RequirementsController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    public async Task<IActionResult> Index(string? search, RequirementStatus? status, Priority? priority)
    {
        var query = AuthorizedRequirements().Include(r => r.Client).Include(r => r.Project).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(r => r.Title.Contains(search) || r.Client!.CompanyName.Contains(search));
        if (status.HasValue) query = query.Where(r => r.Status == status);
        if (priority.HasValue) query = query.Where(r => r.Priority == priority);
        return View(await query.OrderByDescending(r => r.Priority).ThenBy(r => r.DueDate).ToListAsync());
    }

    [Authorize(Roles = "Administrator,Project Manager,Client")]
    public async Task<IActionResult> Create()
    {
        await PopulateLookupsAsync(); return View(new Requirement { CreatedAtUtc = DateTime.UtcNow });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager,Client")]
    public async Task<IActionResult> Create(Requirement requirement)
    {
        if (User.IsInRole("Client"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownClient = await context.Clients.SingleOrDefaultAsync(c => c.ApplicationUserId == userId);
            if (ownClient is null) return Forbid();
            requirement.ClientId = ownClient.Id; requirement.Status = RequirementStatus.New; requirement.AssignedManager = null;
        }
        if (!ModelState.IsValid) { await PopulateLookupsAsync(requirement.ClientId, requirement.ProjectId); return View(requirement); }
        context.Requirements.Add(requirement); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Requirement Created", "Requirement", requirement.Id.ToString(), requirement.Title);
        TempData["Success"] = "Requirement recorded for analysis."; return RedirectToAction(nameof(Details), new { id = requirement.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var requirement = await AuthorizedRequirements().Include(r => r.Client).Include(r => r.Project).Include(r => r.Analysis).Include(r => r.Tasks).ThenInclude(t => t.Developer).FirstOrDefaultAsync(r => r.Id == id);
        return requirement is null ? NotFound() : View(requirement);
    }

    [Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var requirement = await context.Requirements.FindAsync(id); if (requirement is null) return NotFound(); await PopulateLookupsAsync(requirement.ClientId, requirement.ProjectId); return View(requirement);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> Edit(int id, Requirement requirement)
    {
        if (id != requirement.Id) return BadRequest();
        if (!ModelState.IsValid) { await PopulateLookupsAsync(requirement.ClientId, requirement.ProjectId); return View(requirement); }
        context.Update(requirement); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Requirement Updated", "Requirement", requirement.Id.ToString(), requirement.Title);
        TempData["Success"] = "Requirement updated."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> SaveAnalysis(RequirementAnalysis analysis)
    {
        var requirement = await context.Requirements.FindAsync(analysis.RequirementId); if (requirement is null) return NotFound();
        var existing = await context.RequirementAnalyses.SingleOrDefaultAsync(a => a.RequirementId == analysis.RequirementId);
        if (existing is null) context.RequirementAnalyses.Add(analysis); else { analysis.Id = existing.Id; context.Entry(existing).CurrentValues.SetValues(analysis); }
        await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Requirement Analysis Saved", "Requirement", analysis.RequirementId.ToString(), requirement.Title);
        TempData["Success"] = "Analysis saved."; return RedirectToAction(nameof(Details), new { id = analysis.RequirementId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrator,Project Manager")]
    public async Task<IActionResult> ReviewAnalysis(int requirementId, AnalysisDecision decision)
    {
        var analysis = await context.RequirementAnalyses.Include(a => a.Requirement).SingleOrDefaultAsync(a => a.RequirementId == requirementId); if (analysis is null) return NotFound();
        analysis.Decision = decision; analysis.ReviewedAtUtc = DateTime.UtcNow; analysis.ReviewedBy = User.Identity?.Name;
        analysis.Requirement!.Status = decision == AnalysisDecision.Approved ? RequirementStatus.Approved : RequirementStatus.Rejected;
        await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", decision == AnalysisDecision.Approved ? "Requirement Approved" : "Requirement Rejected", "Requirement", requirementId.ToString(), analysis.Requirement.Title);
        TempData["Success"] = $"Analysis {decision.ToString().ToLowerInvariant()}."; return RedirectToAction(nameof(Details), new { id = requirementId });
    }

    private IQueryable<Requirement> AuthorizedRequirements()
    {
        var query = context.Requirements.AsQueryable();
        if (User.IsInRole("Client")) { var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); query = query.Where(r => r.Client!.ApplicationUserId == userId); }
        return query;
    }

    private async Task PopulateLookupsAsync(int? clientId = null, int? projectId = null)
    {
        ViewBag.Clients = new SelectList(await context.Clients.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", clientId);
        ViewBag.Projects = new SelectList(await context.Projects.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", projectId);
    }
}

