using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class RequirementAnalysis
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
    [Required, StringLength(2500)] public string BusinessRequirement { get; set; } = string.Empty;
    [Required, StringLength(1500)] public string BusinessObjective { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string FunctionalRequirements { get; set; } = string.Empty;
    [StringLength(2500)] public string? NonFunctionalRequirements { get; set; }
    [Required, StringLength(3000)] public string AcceptanceCriteria { get; set; } = string.Empty;
    [StringLength(2000)] public string? Dependencies { get; set; }
    [StringLength(2000)] public string? Risks { get; set; }
    [Range(1, 1000)] public int EstimatedHours { get; set; }
    [StringLength(4000)] public string? TechnicalNotes { get; set; }
    public AnalysisDecision Decision { get; set; } = AnalysisDecision.Draft;
    [StringLength(120)] public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
}
