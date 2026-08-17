using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Sprint
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    [Required, StringLength(140)] public string Name { get; set; } = string.Empty;
    [StringLength(1000)] public string? Goal { get; set; }
    [DataType(DataType.Date)] public DateTime StartDate { get; set; }
    [DataType(DataType.Date)] public DateTime EndDate { get; set; }
    public SprintStatus Status { get; set; } = SprintStatus.Planned;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
