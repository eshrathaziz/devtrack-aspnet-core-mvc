using DevTrack.Data;
using DevTrack.Models;
using DevTrack.ViewModels;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Services;

public class DashboardService(ApplicationDbContext context) : IDashboardService
{
    public async Task<DashboardViewModel> GetDashboardAsync(string? userId, bool clientOnly)
    {
        var projects = context.Projects.AsNoTracking().Include(p => p.Client).AsQueryable();
        if (clientOnly && userId is not null) projects = projects.Where(p => p.Client!.ApplicationUserId == userId);
        var ids = projects.Select(p => p.Id);
        var sprintTasks = context.TaskItems.Where(t => t.Sprint!.Status == SprintStatus.Active && ids.Contains(t.ProjectId));
        var total = await sprintTasks.CountAsync();
        var done = await sprintTasks.CountAsync(t => t.Status == TaskStatus.Done);
        return new DashboardViewModel
        {
            ActiveProjects = await projects.CountAsync(p => p.Status == ProjectStatus.Active),
            OpenRequirements = await context.Requirements.CountAsync(r => r.Status != RequirementStatus.Completed && r.Status != RequirementStatus.Rejected && ids.Contains(r.ProjectId ?? -1)),
            ActiveSprints = await context.Sprints.CountAsync(s => s.Status == SprintStatus.Active && ids.Contains(s.ProjectId)),
            TasksInProgress = await context.TaskItems.CountAsync(t => t.Status == TaskStatus.InProgress && ids.Contains(t.ProjectId)),
            UnassignedTasks = await context.TaskItems.CountAsync(t => t.DeveloperProfileId == null && ids.Contains(t.ProjectId)),
            OpenBugs = await context.Bugs.CountAsync(b => b.Status != BugStatus.Closed && ids.Contains(b.ProjectId)),
            CriticalBugs = await context.Bugs.CountAsync(b => b.Severity == BugSeverity.Critical && b.Status != BugStatus.Closed && ids.Contains(b.ProjectId)),
            CompletedTasks = await context.TaskItems.CountAsync(t => t.Status == TaskStatus.Done && ids.Contains(t.ProjectId)),
            SprintCompletionRate = total == 0 ? 0 : Math.Round(done * 100m / total, 1),
            PriorityProjects = await projects.Where(p => p.Status == ProjectStatus.Active).OrderByDescending(p => p.Priority).ThenBy(p => p.EndDate).Take(4).ToListAsync(),
            MyWork = await context.TaskItems.Include(t => t.Project).Include(t => t.Developer).Where(t => ids.Contains(t.ProjectId) && t.Status != TaskStatus.Done).OrderBy(t => t.DueDate).Take(6).ToListAsync(),
            RecentActivity = await context.ActivityLogs.OrderByDescending(a => a.CreatedAtUtc).Take(8).ToListAsync(),
            ProjectsByStatus = await projects.GroupBy(p => p.Status).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count()),
            TasksByStatus = await context.TaskItems.Where(t => ids.Contains(t.ProjectId)).GroupBy(t => t.Status).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count()),
            BugsBySeverity = await context.Bugs.Where(b => ids.Contains(b.ProjectId)).GroupBy(b => b.Severity).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count())
        };
    }
}
