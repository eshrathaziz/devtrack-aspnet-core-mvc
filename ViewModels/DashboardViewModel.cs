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
    public decimal SprintCompletionRate { get; set; }
    public IReadOnlyList<Project> PriorityProjects { get; set; } = [];
    public IReadOnlyList<TaskItem> MyWork { get; set; } = [];
    public IReadOnlyList<ActivityLog> RecentActivity { get; set; } = [];
    public Dictionary<string, int> ProjectsByStatus { get; set; } = [];
    public Dictionary<string, int> TasksByStatus { get; set; } = [];
    public Dictionary<string, int> BugsBySeverity { get; set; } = [];
}
