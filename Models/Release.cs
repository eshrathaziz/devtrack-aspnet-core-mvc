using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Release
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    [Required, StringLength(30)] public string Version { get; set; } = string.Empty;
    [Required, StringLength(160)] public string Name { get; set; } = string.Empty;
    [DataType(DataType.Date)] public DateTime ReleaseDate { get; set; }
    public ReleaseStatus Status { get; set; } = ReleaseStatus.Planned;
    [StringLength(3500)] public string? Notes { get; set; }
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}
