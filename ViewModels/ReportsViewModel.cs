using DevTrack.Models;

namespace DevTrack.ViewModels;

public class ReportsViewModel
{
    public IReadOnlyList<Project> Projects { get; set; } = [];
    public IReadOnlyList<ActivityLog> Activities { get; set; } = [];
    public Dictionary<string, int> TaskStatusCounts { get; set; } = [];
    public Dictionary<string, int> BugSeverityCounts { get; set; } = [];
}
