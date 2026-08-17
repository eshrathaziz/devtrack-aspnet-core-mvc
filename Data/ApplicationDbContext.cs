using DevTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<RequirementAnalysis> RequirementAnalyses => Set<RequirementAnalysis>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<DeveloperProfile> DeveloperProfiles => Set<DeveloperProfile>();
    public DbSet<Bug> Bugs => Set<Bug>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ClientCommunication> ClientCommunications => Set<ClientCommunication>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Client>().HasIndex(c => c.CompanyName).IsUnique();
        builder.Entity<Project>().HasIndex(p => new { p.ClientId, p.Name }).IsUnique();
        builder.Entity<Release>().HasIndex(r => new { r.ProjectId, r.Version }).IsUnique();
        builder.Entity<RequirementAnalysis>().HasIndex(a => a.RequirementId).IsUnique();
        builder.Entity<Project>().HasOne(p => p.Client).WithMany(c => c.Projects).HasForeignKey(p => p.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Requirement>().HasOne(r => r.Client).WithMany(c => c.Requirements).HasForeignKey(r => r.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Requirement>().HasOne(r => r.Project).WithMany(p => p.Requirements).HasForeignKey(r => r.ProjectId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Sprint>().HasOne(s => s.Project).WithMany(p => p.Sprints).HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TaskItem>().HasOne(t => t.Project).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TaskItem>().HasOne(t => t.Sprint).WithMany(s => s.Tasks).HasForeignKey(t => t.SprintId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<TaskItem>().HasOne(t => t.Requirement).WithMany(r => r.Tasks).HasForeignKey(t => t.RequirementId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<TaskItem>().HasOne(t => t.Developer).WithMany(d => d.AssignedTasks).HasForeignKey(t => t.DeveloperProfileId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Bug>().HasOne(b => b.Project).WithMany(p => p.Bugs).HasForeignKey(b => b.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Bug>().HasOne(b => b.AssignedDeveloper).WithMany(d => d.AssignedBugs).HasForeignKey(b => b.AssignedDeveloperId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Bug>().HasOne(b => b.Release).WithMany(r => r.Bugs).HasForeignKey(b => b.ReleaseId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Release>().HasOne(r => r.Project).WithMany(p => p.Releases).HasForeignKey(r => r.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<DeveloperProfile>().HasOne(d => d.ApplicationUser).WithOne(u => u.DeveloperProfile).HasForeignKey<DeveloperProfile>(d => d.ApplicationUserId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Client>().HasOne(c => c.ApplicationUser).WithOne(u => u.ClientAccount).HasForeignKey<Client>(c => c.ApplicationUserId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Notification>().HasOne(n => n.ApplicationUser).WithMany(u => u.Notifications).HasForeignKey(n => n.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Requirement>().Property(r => r.Type).HasConversion<string>();
        builder.Entity<Requirement>().Property(r => r.Status).HasConversion<string>();
        builder.Entity<Project>().Property(p => p.Status).HasConversion<string>();
        builder.Entity<TaskItem>().Property(t => t.Status).HasConversion<string>();
        builder.Entity<TaskItem>().Property(t => t.EstimatedHours).HasPrecision(10, 2);
        builder.Entity<TaskItem>().Property(t => t.ActualHours).HasPrecision(10, 2);
        builder.Entity<Bug>().Property(b => b.Status).HasConversion<string>();
    }
}
