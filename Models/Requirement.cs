using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Requirement
{
    public int Id { get; set; }
    [Required, StringLength(160)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public RequirementType Type { get; set; } = RequirementType.Functional;
    public Priority Priority { get; set; } = Priority.Medium;
    public RequirementStatus Status { get; set; } = RequirementStatus.New;
    [Range(1, 10)] public int BusinessValue { get; set; } = 5;
    [StringLength(120)] public string? AssignedManager { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    [DataType(DataType.Date)] public DateTime? DueDate { get; set; }
    public RequirementAnalysis? Analysis { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<ClientCommunication> Communications { get; set; } = new List<ClientCommunication>();
}
