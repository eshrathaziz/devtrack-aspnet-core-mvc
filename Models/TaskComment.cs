using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class TaskComment
{
    public int Id { get; set; }
    public int? TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public int? BugId { get; set; }
    public Bug? Bug { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    [Required, StringLength(120)] public string AuthorName { get; set; } = string.Empty;
    [Required, StringLength(2500)] public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
