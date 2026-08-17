using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Bug
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
    public BugSeverity Severity { get; set; } = BugSeverity.Medium;
    public Priority Priority { get; set; } = Priority.Medium;
    public int? AssignedDeveloperId { get; set; }
    public DeveloperProfile? AssignedDeveloper { get; set; }
    [StringLength(120)] public string? Reporter { get; set; }
    public BugStatus Status { get; set; } = BugStatus.Open;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
    public int? ReleaseId { get; set; }
    public Release? Release { get; set; }
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
