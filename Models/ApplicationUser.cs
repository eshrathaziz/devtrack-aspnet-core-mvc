using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)] public string? DisplayName { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DeveloperProfile? DeveloperProfile { get; set; }
    public Client? ClientAccount { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
