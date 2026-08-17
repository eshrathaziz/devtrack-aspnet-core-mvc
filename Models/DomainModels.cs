using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DevTrack.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(120)] public string DisplayName { get; set; } = "";
    [MaxLength(80)] public string RoleLabel { get; set; } = "Developer";
    public string Initials => string.Join("", (DisplayName ?? "User").Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(x => x[0])).ToUpperInvariant();
}

public enum ProjectStatus { Planning, Active, OnHold, Completed, Cancelled }
public enum Priority { Low, Medium, High, Critical }
public enum RequirementStatus { New, UnderAnalysis, ClientReview, Approved, Rejected, InDevelopment, Completed }
public enum RequirementType { Functional, NonFunctional, Business, Technical, ChangeRequest }
public enum SprintStatus { Planned, Active, Completed }
public enum TaskStatus { Backlog, ToDo, InProgress, CodeReview, Testing, Done, Blocked }
public enum BugSeverity { Low, Medium, High, Critical }
public enum BugStatus { Open, Assigned, InProgress, Fixed, Testing, Closed, Reopened }
public enum ReleaseStatus { Planned, InDevelopment, Testing, Released, Cancelled }

public class Client
{
    public int Id { get; set; }
    [MaxLength(20)] public string ClientCode { get; set; } = "";
    [Required, MaxLength(150)] public string CompanyName { get; set; } = "";
    [Required, MaxLength(120)] public string ContactPerson { get; set; } = "";
    [Required, EmailAddress, MaxLength(180)] public string Email { get; set; } = "";
    [MaxLength(40)] public string Phone { get; set; } = "";
    [MaxLength(100)] public string Industry { get; set; } = "";
    [MaxLength(200)] public string City { get; set; } = "";
    [MaxLength(80)] public string Country { get; set; } = "";
    [MaxLength(40)] public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();
}

public class Project
{
    public int Id { get; set; }
    [MaxLength(20)] public string ProjectCode { get; set; } = "";
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    [MaxLength(500)] public string Description { get; set; } = "";
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    [MaxLength(120)] public string ProjectManager { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public Priority Priority { get; set; } = Priority.Medium;
    public int Progress { get; set; }
    public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();
    public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
    public ICollection<Release> Releases { get; set; } = new List<Release>();
}

public class Requirement
{
    public int Id { get; set; }
    [MaxLength(20)] public string RequirementCode { get; set; } = "";
    [Required, MaxLength(180)] public string Title { get; set; } = "";
    [MaxLength(1000)] public string Description { get; set; } = "";
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public RequirementType Type { get; set; } = RequirementType.Functional;
    public Priority Priority { get; set; } = Priority.Medium;
    public RequirementStatus Status { get; set; } = RequirementStatus.New;
    public int BusinessValue { get; set; }
    [MaxLength(120)] public string AssignedManager { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public RequirementAnalysis? Analysis { get; set; }
}

public class RequirementAnalysis
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    [MaxLength(1000)] public string BusinessRequirement { get; set; } = "";
    [MaxLength(1000)] public string BusinessObjective { get; set; } = "";
    [MaxLength(1500)] public string FunctionalRequirements { get; set; } = "";
    [MaxLength(1500)] public string NonFunctionalRequirements { get; set; } = "";
    [MaxLength(1200)] public string AcceptanceCriteria { get; set; } = "";
    [MaxLength(1000)] public string Dependencies { get; set; } = "";
    [MaxLength(1000)] public string Risks { get; set; } = "";
    public int EstimatedEffort { get; set; }
    [MaxLength(1200)] public string TechnicalNotes { get; set; } = "";
    public bool Approved { get; set; }
}

public class Sprint
{
    public int Id { get; set; }
    [MaxLength(20)] public string SprintCode { get; set; } = "";
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    [MaxLength(500)] public string Goal { get; set; } = "";
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SprintStatus Status { get; set; } = SprintStatus.Planned;
    public int StoryPoints { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

public class DeveloperProfile
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Name { get; set; } = "";
    [MaxLength(250)] public string Skills { get; set; } = "";
    [MaxLength(50)] public string ExperienceLevel { get; set; } = "";
    public int CurrentWorkload { get; set; }
    [MaxLength(40)] public string Availability { get; set; } = "Available";
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

public class TaskItem
{
    public int Id { get; set; }
    [MaxLength(20)] public string TaskCode { get; set; } = "";
    [Required, MaxLength(180)] public string Title { get; set; } = "";
    [MaxLength(1000)] public string Description { get; set; } = "";
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int SprintId { get; set; }
    public Sprint? Sprint { get; set; }
    public int? RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    public int? DeveloperProfileId { get; set; }
    public DeveloperProfile? Developer { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public int StoryPoints { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Bug
{
    public int Id { get; set; }
    [MaxLength(20)] public string BugCode { get; set; } = "";
    [Required, MaxLength(180)] public string Title { get; set; } = "";
    [MaxLength(1000)] public string Description { get; set; } = "";
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? TaskItemId { get; set; }
    public TaskItem? Task { get; set; }
    public int? DeveloperProfileId { get; set; }
    public DeveloperProfile? Developer { get; set; }
    public BugSeverity Severity { get; set; } = BugSeverity.Medium;
    public Priority Priority { get; set; } = Priority.Medium;
    public BugStatus Status { get; set; } = BugStatus.Open;
    [MaxLength(120)] public string Reporter { get; set; } = "Quality Engineering";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}

public class Release
{
    public int Id { get; set; }
    [MaxLength(20)] public string ReleaseCode { get; set; } = "";
    [Required, MaxLength(30)] public string Version { get; set; } = "";
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ReleaseStatus Status { get; set; } = ReleaseStatus.Planned;
    [MaxLength(1000)] public string Notes { get; set; } = "";
}

public class ActivityLog
{
    public int Id { get; set; }
    [MaxLength(120)] public string UserName { get; set; } = "System User";
    [MaxLength(80)] public string Action { get; set; } = "Updated";
    [MaxLength(80)] public string Entity { get; set; } = "Project";
    [MaxLength(30)] public string EntityId { get; set; } = "";
    [MaxLength(500)] public string Description { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class Notification
{
    public int Id { get; set; }
    [MaxLength(120)] public string UserId { get; set; } = "";
    [MaxLength(200)] public string Title { get; set; } = "";
    [MaxLength(500)] public string Message { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Communication
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    [MaxLength(60)] public string Type { get; set; } = "Meeting";
    [MaxLength(180)] public string Subject { get; set; } = "";
    [MaxLength(1000)] public string Notes { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
