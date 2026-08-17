using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? SprintId { get; set; }
    public Sprint? Sprint { get; set; }
    public int? RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [StringLength(4000)] public string? Description { get; set; }
    public int? DeveloperProfileId { get; set; }
    public DeveloperProfile? Developer { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;
    [Range(0, 100)] public int StoryPoints { get; set; }
    [Range(0, 1000)] public decimal EstimatedHours { get; set; }
    [Range(0, 1000)] public decimal ActualHours { get; set; }
    [DataType(DataType.Date)] public DateTime? DueDate { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}
