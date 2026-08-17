using DevTrack.Models;

namespace DevTrack.ViewModels;

public class DashboardViewModel
{
    public int ActiveProjects { get; set; }
    public int OpenRequirements { get; set; }
    public int ActiveSprints { get; set; }
    public int TasksInProgress { get; set; }
    public int UnassignedTasks { get; set; }
    public int OpenBugs { get; set; }
    public int CriticalBugs { get; set; }
    public int CompletedTasks { get; set; }
    public int SprintCompletionRate { get; set; }
    public List<ProjectSummary> ProjectSummaries { get; set; } = new();
    public List<TaskItem> RecentTasks { get; set; } = new();
    public List<Bug> PriorityBugs { get; set; } = new();
    public List<ActivityLog> RecentActivity { get; set; } = new();
    public List<SprintSummary> SprintSummaries { get; set; } = new();
    public Dictionary<string, int> ProjectStatusData { get; set; } = new();
    public Dictionary<string, int> TaskStatusData { get; set; } = new();
    public Dictionary<string, int> BugSeverityData { get; set; } = new();
}

public record ProjectSummary(string Code, string Name, string Client, ProjectStatus Status, Priority Priority, int Progress, string Manager);
public record SprintSummary(string Name, string Project, int Progress, int Completed, int Total, int Points, int DaysRemaining);

public class ModuleListViewModel<T>
{
    public string Search { get; set; } = "";
    public string Status { get; set; } = "All statuses";
    public List<T> Items { get; set; } = new();
}
