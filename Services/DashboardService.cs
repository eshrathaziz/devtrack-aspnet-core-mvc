using DevTrack.Data;
using DevTrack.Models;
using DevTrack.ViewModels;
using Microsoft.EntityFrameworkCore;
using WorkStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Services;

public class DashboardService(ApplicationDbContext db)
{
    public async Task<DashboardViewModel> BuildAsync()
    {
        var openRequirementStatuses = new[] { RequirementStatus.New, RequirementStatus.UnderAnalysis, RequirementStatus.ClientReview };
        var openBugStatuses = new[] { BugStatus.Open, BugStatus.Assigned, BugStatus.InProgress, BugStatus.Reopened };
        var allSprints = await db.Sprints.Include(x => x.Tasks).Include(x => x.Project).ToListAsync();
        var model = new DashboardViewModel
        {
            ActiveProjects = await db.Projects.CountAsync(x => x.Status == ProjectStatus.Active),
            OpenRequirements = await db.Requirements.CountAsync(x => openRequirementStatuses.Contains(x.Status)),
            ActiveSprints = await db.Sprints.CountAsync(x => x.Status == SprintStatus.Active),
            TasksInProgress = await db.Tasks.CountAsync(x => x.Status == WorkStatus.InProgress),
            UnassignedTasks = await db.Tasks.CountAsync(x => x.DeveloperProfileId == null),
            OpenBugs = await db.Bugs.CountAsync(x => openBugStatuses.Contains(x.Status)),
            CriticalBugs = await db.Bugs.CountAsync(x => x.Severity == BugSeverity.Critical && x.Status != BugStatus.Closed),
            CompletedTasks = await db.Tasks.CountAsync(x => x.Status == WorkStatus.Done),
            ProjectSummaries = await db.Projects.Include(x => x.Client).OrderByDescending(x => x.Progress).Take(5).Select(x => new ProjectSummary(x.ProjectCode, x.Name, x.Client!.CompanyName, x.Status, x.Priority, x.Progress, x.ProjectManager)).ToListAsync(),
            RecentTasks = await db.Tasks.Include(x => x.Project).Include(x => x.Developer).OrderByDescending(x => x.UpdatedAt).Take(6).ToListAsync(),
            PriorityBugs = await db.Bugs.Include(x => x.Project).OrderByDescending(x => x.Severity).ThenByDescending(x => x.CreatedAt).Take(4).ToListAsync(),
            RecentActivity = await db.ActivityLogs.OrderByDescending(x => x.Timestamp).Take(5).ToListAsync(),
            ProjectStatusData = await db.Projects.GroupBy(x => x.Status).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count()),
            TaskStatusData = await db.Tasks.GroupBy(x => x.Status).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count()),
            BugSeverityData = await db.Bugs.GroupBy(x => x.Severity).ToDictionaryAsync(g => g.Key.ToString(), g => g.Count())
        };
        var totalSprints = allSprints.Count;
        model.SprintCompletionRate = totalSprints == 0 ? 0 : (int)Math.Round(allSprints.Count(x => x.Status == SprintStatus.Completed) * 100.0 / totalSprints);
        model.SprintSummaries = allSprints.Where(x => x.Status != SprintStatus.Completed).Take(3).Select(x =>
        {
            var total = x.Tasks.Count;
            var completed = x.Tasks.Count(t => t.Status == WorkStatus.Done);
            var progress = total == 0 ? 0 : completed * 100 / total;
            return new SprintSummary(x.Name, x.Project?.Name ?? "Project", progress, completed, total, x.StoryPoints, Math.Max(0, (x.EndDate.Date - DateTime.UtcNow.Date).Days));
        }).ToList();
        return model;
    }
}

public class ActivityService(ApplicationDbContext db)
{
    public async Task RecordAsync(string user, string action, string entity, string entityId, string description)
    {
        db.ActivityLogs.Add(new ActivityLog { UserName = user, Action = action, Entity = entity, EntityId = entityId, Description = description });
        await db.SaveChangesAsync();
    }
}
