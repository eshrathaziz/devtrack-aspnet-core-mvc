using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class DeveloperProfile
{
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    [Required, StringLength(160)] public string DisplayName { get; set; } = string.Empty;
    [StringLength(700)] public string? Skills { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; } = ExperienceLevel.Junior;
    [Range(0, 100)] public int CurrentWorkload { get; set; }
    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Available;
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<Bug> AssignedBugs { get; set; } = new List<Bug>();
}
