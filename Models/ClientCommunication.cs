using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class ClientCommunication
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    public CommunicationType Type { get; set; }
    [Required, StringLength(200)] public string Subject { get; set; } = string.Empty;
    [Required, StringLength(3000)] public string Description { get; set; } = string.Empty;
    [StringLength(400)] public string? Participants { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? FollowUpDateUtc { get; set; }
    [StringLength(120)] public string? Employee { get; set; }
    [StringLength(2000)] public string? Notes { get; set; }
}
