using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Notification
{
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public NotificationType Type { get; set; }
    [Required, StringLength(180)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(700)] public string Message { get; set; } = string.Empty;
    [StringLength(300)] public string? Link { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
