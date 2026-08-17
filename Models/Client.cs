using System.ComponentModel.DataAnnotations;

namespace DevTrack.Models;

public class Client
{
    public int Id { get; set; }
    [Required, StringLength(160), Display(Name = "Company name")] public string CompanyName { get; set; } = string.Empty;
    [Required, StringLength(120), Display(Name = "Contact person")] public string ContactPerson { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(160)] public string Email { get; set; } = string.Empty;
    [Phone, StringLength(40)] public string? Phone { get; set; }
    [StringLength(100)] public string? Industry { get; set; }
    [StringLength(220)] public string? Address { get; set; }
    [StringLength(80)] public string? City { get; set; }
    [StringLength(80)] public string? Country { get; set; }
    public ClientStatus Status { get; set; } = ClientStatus.Active;
    [StringLength(120)] public string? AccountManager { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();
    public ICollection<ClientCommunication> Communications { get; set; } = new List<ClientCommunication>();
}
