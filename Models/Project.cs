using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Project
{
    public int Id { get; set; }
    [Required, StringLength(160), Display(Name = "Project name")] public string Name { get; set; } = string.Empty;
    [StringLength(1200)] public string? Description { get; set; }
    [Required, Display(Name = "Client")] public int ClientId { get; set; }
    public Client? Client { get; set; }
    [StringLength(120), Display(Name = "Project manager")] public string? ProjectManager { get; set; }
    [DataType(DataType.Date)] public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    [DataType(DataType.Date)] public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public Priority Priority { get; set; } = Priority.Medium;
    [Range(0, 100)] public int Progress { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();
    public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
    public ICollection<Release> Releases { get; set; } = new List<Release>();
    public ICollection<ClientCommunication> Communications { get; set; } = new List<ClientCommunication>();
}
