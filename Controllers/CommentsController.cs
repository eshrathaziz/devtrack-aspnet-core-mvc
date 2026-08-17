using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Controllers;

[Authorize]
public class CommentsController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TaskComment input)
    {
        if (!ModelState.IsValid || (input.TaskItemId is null && input.BugId is null && input.ProjectId is null && input.RequirementId is null)) return BadRequest(new { message = "A comment and related record are required." });
        input.AuthorName = User.Identity?.Name ?? "User"; input.CreatedAtUtc = DateTime.UtcNow;
        context.TaskComments.Add(input); await context.SaveChangesAsync();
        await activityLog.RecordAsync(input.AuthorName, "Comment Added", "Comment", input.Id.ToString(), "Added a collaboration comment.");
        return Json(new { id = input.Id, author = input.AuthorName, comment = input.Comment, createdAt = input.CreatedAtUtc.ToString("u") });
    }
}
