using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class ActivityLog
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string UserName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Action { get; set; } = string.Empty;
    [Required, StringLength(100)] public string EntityName { get; set; } = string.Empty;
    [StringLength(80)] public string? EntityId { get; set; }
    [Required, StringLength(1000)] public string Description { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
