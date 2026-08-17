using DevTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<RequirementAnalysis> RequirementAnalyses => Set<RequirementAnalysis>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<DeveloperProfile> Developers => Set<DeveloperProfile>();
    public DbSet<Bug> Bugs => Set<Bug>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Communication> Communications => Set<Communication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Client>().HasIndex(x => x.ClientCode).IsUnique();
        modelBuilder.Entity<Project>().HasIndex(x => x.ProjectCode).IsUnique();
        modelBuilder.Entity<Requirement>().HasIndex(x => x.RequirementCode).IsUnique();
        modelBuilder.Entity<TaskItem>().HasIndex(x => x.TaskCode).IsUnique();
        modelBuilder.Entity<Bug>().HasIndex(x => x.BugCode).IsUnique();
        modelBuilder.Entity<Release>().HasIndex(x => x.ReleaseCode).IsUnique();
        modelBuilder.Entity<RequirementAnalysis>().HasOne(x => x.Requirement).WithOne(x => x.Analysis).HasForeignKey<RequirementAnalysis>(x => x.RequirementId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Project>().HasOne(x => x.Client).WithMany(x => x.Projects).HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Requirement>().HasOne(x => x.Project).WithMany(x => x.Requirements).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TaskItem>().HasOne(x => x.Sprint).WithMany(x => x.Tasks).HasForeignKey(x => x.SprintId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TaskItem>().HasOne(x => x.Developer).WithMany(x => x.Tasks).HasForeignKey(x => x.DeveloperProfileId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Bug>().HasOne(x => x.Project).WithMany(x => x.Bugs).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Bug>().HasOne(x => x.Developer).WithMany().HasForeignKey(x => x.DeveloperProfileId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Release>().HasOne(x => x.Project).WithMany(x => x.Releases).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (!context.Clients.Any())
        {
            var clients = new[]
            {
                new Client { ClientCode="CL-104", CompanyName="Apex Health Systems", ContactPerson="Maya Chen", Email="maya.chen@apexhealth.example", Phone="+1 617 555 0134", Industry="Healthcare", City="Boston", Country="United States" },
                new Client { ClientCode="CL-108", CompanyName="NovaCore Logistics", ContactPerson="Ethan Brooks", Email="ethan.brooks@novacore.example", Phone="+1 312 555 0178", Industry="Logistics", City="Chicago", Country="United States" },
                new Client { ClientCode="CL-112", CompanyName="Vertex Financial Services", ContactPerson="Leila Patel", Email="leila.patel@vertexfs.example", Phone="+44 20 5555 0181", Industry="Financial Services", City="London", Country="United Kingdom" },
                new Client { ClientCode="CL-116", CompanyName="BlueOrbit Manufacturing", ContactPerson="Jon Bell", Email="jon.bell@blueorbit.example", Phone="+1 416 555 0119", Industry="Manufacturing", City="Toronto", Country="Canada" },
                new Client { ClientCode="CL-120", CompanyName="GreenField Retail", ContactPerson="Ari Santos", Email="ari.santos@greenfield.example", Phone="+61 2 5550 0155", Industry="Retail", City="Sydney", Country="Australia" },
                new Client { ClientCode="CL-124", CompanyName="Harbor & Pine", ContactPerson="Rosa Miller", Email="rosa.miller@harborpine.example", Phone="+1 206 555 0105", Industry="Hospitality", City="Seattle", Country="United States" },
                new Client { ClientCode="CL-128", CompanyName="Orion Public Works", ContactPerson="Devon Reed", Email="devon.reed@orionworks.example", Phone="+1 303 555 0124", Industry="Public Sector", City="Denver", Country="United States" },
                new Client { ClientCode="CL-132", CompanyName="Cedar Education Group", ContactPerson="Nina Okafor", Email="nina.okafor@cedaredu.example", Phone="+1 416 555 0192", Industry="Education", City="Ottawa", Country="Canada" }
            };
            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();

            var projects = new[]
            {
                new Project { ProjectCode="PRJ-2401", Name="CareConnect Portal", Description="Unified patient engagement and care coordination workspace.", ClientId=clients[0].Id, ProjectManager="Olivia Morgan", StartDate=DateTime.UtcNow.AddMonths(-5), EndDate=DateTime.UtcNow.AddMonths(2), Status=ProjectStatus.Active, Priority=Priority.Critical, Progress=72 },
                new Project { ProjectCode="PRJ-2402", Name="Route Optimizer", Description="Real-time dispatch planning for regional delivery teams.", ClientId=clients[1].Id, ProjectManager="Marcus Lee", StartDate=DateTime.UtcNow.AddMonths(-3), EndDate=DateTime.UtcNow.AddMonths(3), Status=ProjectStatus.Active, Priority=Priority.High, Progress=48 },
                new Project { ProjectCode="PRJ-2403", Name="Investor Workspace", Description="Secure investor reporting and document collaboration platform.", ClientId=clients[2].Id, ProjectManager="Olivia Morgan", StartDate=DateTime.UtcNow.AddMonths(-2), EndDate=DateTime.UtcNow.AddMonths(5), Status=ProjectStatus.Active, Priority=Priority.High, Progress=35 },
                new Project { ProjectCode="PRJ-2404", Name="Factory Pulse", Description="Operational visibility dashboard for production sites.", ClientId=clients[3].Id, ProjectManager="Nikhil Shah", StartDate=DateTime.UtcNow.AddMonths(-8), EndDate=DateTime.UtcNow.AddMonths(-1), Status=ProjectStatus.Completed, Priority=Priority.Medium, Progress=100 },
                new Project { ProjectCode="PRJ-2405", Name="GreenField Commerce", Description="Composable commerce foundation for retail expansion.", ClientId=clients[4].Id, ProjectManager="Marcus Lee", StartDate=DateTime.UtcNow.AddMonths(-1), EndDate=DateTime.UtcNow.AddMonths(7), Status=ProjectStatus.Planning, Priority=Priority.Medium, Progress=12 },
                new Project { ProjectCode="PRJ-2406", Name="Cedar Learning Hub", Description="Modern learning operations and course delivery suite.", ClientId=clients[7].Id, ProjectManager="Nikhil Shah", StartDate=DateTime.UtcNow.AddMonths(-6), EndDate=DateTime.UtcNow.AddMonths(-2), Status=ProjectStatus.OnHold, Priority=Priority.Low, Progress=58 }
            };
            context.Projects.AddRange(projects);
            await context.SaveChangesAsync();

            var developers = new[]
            {
                new DeveloperProfile { Name="Priya Kapoor", Skills="C#, ASP.NET Core, SQL Server", ExperienceLevel="Senior", CurrentWorkload=82, Availability="At capacity" },
                new DeveloperProfile { Name="Daniel Kim", Skills="JavaScript, jQuery, UI systems", ExperienceLevel="Mid-level", CurrentWorkload=64, Availability="Available soon" },
                new DeveloperProfile { Name="Sofia Alvarez", Skills="QA automation, Cypress, API testing", ExperienceLevel="Senior", CurrentWorkload=56, Availability="Available" },
                new DeveloperProfile { Name="Noah Williams", Skills="EF Core, integrations, Azure", ExperienceLevel="Mid-level", CurrentWorkload=43, Availability="Available" },
                new DeveloperProfile { Name="Amara Okoye", Skills="Razor, accessibility, CSS", ExperienceLevel="Junior", CurrentWorkload=28, Availability="Available" }
            };
            context.Developers.AddRange(developers);
            await context.SaveChangesAsync();

            var sprints = new[]
            {
                new Sprint { SprintCode="SPR-31", Name="Sprint 31 · CareConnect", Goal="Complete appointment reminders and care-team visibility.", ProjectId=projects[0].Id, StartDate=DateTime.UtcNow.AddDays(-8), EndDate=DateTime.UtcNow.AddDays(6), Status=SprintStatus.Active, StoryPoints=42 },
                new Sprint { SprintCode="SPR-32", Name="Sprint 32 · Route Optimizer", Goal="Ship route constraints and driver exception flows.", ProjectId=projects[1].Id, StartDate=DateTime.UtcNow.AddDays(-3), EndDate=DateTime.UtcNow.AddDays(11), Status=SprintStatus.Active, StoryPoints=34 },
                new Sprint { SprintCode="SPR-33", Name="Sprint 33 · Investor Workspace", Goal="Establish the secure reporting shell.", ProjectId=projects[2].Id, StartDate=DateTime.UtcNow.AddDays(4), EndDate=DateTime.UtcNow.AddDays(18), Status=SprintStatus.Planned, StoryPoints=28 },
                new Sprint { SprintCode="SPR-30", Name="Sprint 30 · Factory Pulse", Goal="Close production insights MVP.", ProjectId=projects[3].Id, StartDate=DateTime.UtcNow.AddDays(-42), EndDate=DateTime.UtcNow.AddDays(-28), Status=SprintStatus.Completed, StoryPoints=38 }
            };
            context.Sprints.AddRange(sprints);
            await context.SaveChangesAsync();

            var requirements = new[]
            {
                new Requirement { RequirementCode="REQ-418", Title="Care-team appointment reminders", Description="Patients and assigned coordinators need configurable reminders before an appointment.", ClientId=clients[0].Id, ProjectId=projects[0].Id, Type=RequirementType.Functional, Priority=Priority.High, Status=RequirementStatus.InDevelopment, BusinessValue=9, AssignedManager="Olivia Morgan", DueDate=DateTime.UtcNow.AddDays(10) },
                new Requirement { RequirementCode="REQ-419", Title="Single patient timeline", Description="A consolidated timeline should display clinical and administrative interactions.", ClientId=clients[0].Id, ProjectId=projects[0].Id, Type=RequirementType.Business, Priority=Priority.Critical, Status=RequirementStatus.Approved, BusinessValue=10, AssignedManager="Olivia Morgan", DueDate=DateTime.UtcNow.AddDays(22) },
                new Requirement { RequirementCode="REQ-405", Title="Driver exception workflow", Description="Dispatchers need to record missed stops and re-route impacted deliveries.", ClientId=clients[1].Id, ProjectId=projects[1].Id, Type=RequirementType.Functional, Priority=Priority.High, Status=RequirementStatus.UnderAnalysis, BusinessValue=8, AssignedManager="Marcus Lee", DueDate=DateTime.UtcNow.AddDays(13) },
                new Requirement { RequirementCode="REQ-399", Title="Investor document permissions", Description="Document access must be scoped by fund, role, and reporting period.", ClientId=clients[2].Id, ProjectId=projects[2].Id, Type=RequirementType.NonFunctional, Priority=Priority.Critical, Status=RequirementStatus.ClientReview, BusinessValue=10, AssignedManager="Olivia Morgan", DueDate=DateTime.UtcNow.AddDays(18) },
                new Requirement { RequirementCode="REQ-388", Title="Production line status feed", Description="Supervisors need a near-real-time view of line state and downtime reason.", ClientId=clients[3].Id, ProjectId=projects[3].Id, Type=RequirementType.Technical, Priority=Priority.Medium, Status=RequirementStatus.Completed, BusinessValue=7, AssignedManager="Nikhil Shah", DueDate=DateTime.UtcNow.AddDays(-32) }
            };
            context.Requirements.AddRange(requirements);
            await context.SaveChangesAsync();

            var tasks = new[]
            {
                new TaskItem { TaskCode="TASK-782", Title="Reminder preference controls", Description="Add configurable cadence and opt-out behavior.", ProjectId=projects[0].Id, SprintId=sprints[0].Id, RequirementId=requirements[0].Id, DeveloperProfileId=developers[0].Id, Priority=Priority.High, Status=WorkStatus.InProgress, StoryPoints=5, EstimatedHours=14, ActualHours=9, DueDate=DateTime.UtcNow.AddDays(3) },
                new TaskItem { TaskCode="TASK-783", Title="Timeline event grouping", Description="Group patient events by date with audit context.", ProjectId=projects[0].Id, SprintId=sprints[0].Id, RequirementId=requirements[1].Id, DeveloperProfileId=developers[3].Id, Priority=Priority.Critical, Status=WorkStatus.CodeReview, StoryPoints=8, EstimatedHours=18, ActualHours=15, DueDate=DateTime.UtcNow.AddDays(2) },
                new TaskItem { TaskCode="TASK-768", Title="Route constraint rules", Description="Implement driver and depot constraints.", ProjectId=projects[1].Id, SprintId=sprints[1].Id, RequirementId=requirements[2].Id, DeveloperProfileId=developers[1].Id, Priority=Priority.High, Status=WorkStatus.Testing, StoryPoints=8, EstimatedHours=20, ActualHours=18, DueDate=DateTime.UtcNow.AddDays(1) },
                new TaskItem { TaskCode="TASK-741", Title="Permission matrix", Description="Create scoped access policy for investor documents.", ProjectId=projects[2].Id, SprintId=sprints[2].Id, RequirementId=requirements[3].Id, DeveloperProfileId=developers[0].Id, Priority=Priority.Critical, Status=WorkStatus.ToDo, StoryPoints=5, EstimatedHours=16, DueDate=DateTime.UtcNow.AddDays(12) },
                new TaskItem { TaskCode="TASK-709", Title="Line status ingestion", Description="Consume production status events and persist snapshots.", ProjectId=projects[3].Id, SprintId=sprints[3].Id, RequirementId=requirements[4].Id, DeveloperProfileId=developers[3].Id, Priority=Priority.Medium, Status=WorkStatus.Done, StoryPoints=8, EstimatedHours=22, ActualHours=21, DueDate=DateTime.UtcNow.AddDays(-31) },
                new TaskItem { TaskCode="TASK-790", Title="Mobile review pass", Description="Review responsive states with the client operations team.", ProjectId=projects[0].Id, SprintId=sprints[0].Id, Priority=Priority.Medium, Status=WorkStatus.Blocked, StoryPoints=3, EstimatedHours=8, DueDate=DateTime.UtcNow.AddDays(5) }
            };
            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();

            var bugs = new[]
            {
                new Bug { BugCode="BUG-214", Title="Reminder sends twice when timezone changes", Description="Duplicate reminder observed after profile timezone update.", ProjectId=projects[0].Id, TaskItemId=tasks[0].Id, DeveloperProfileId=developers[0].Id, Severity=BugSeverity.High, Priority=Priority.High, Status=BugStatus.InProgress, Reporter="Sofia Alvarez" },
                new Bug { BugCode="BUG-209", Title="Route summary loses depot label", Description="The summary card omits depot label for multi-stop plans.", ProjectId=projects[1].Id, TaskItemId=tasks[2].Id, DeveloperProfileId=developers[1].Id, Severity=BugSeverity.Medium, Priority=Priority.Medium, Status=BugStatus.Testing, Reporter="Sofia Alvarez" },
                new Bug { BugCode="BUG-198", Title="Historical line feed shows blank state", Description="Older status snapshots do not render a fallback label.", ProjectId=projects[3].Id, TaskItemId=tasks[4].Id, Severity=BugSeverity.Low, Priority=Priority.Low, Status=BugStatus.Closed, Reporter="Sofia Alvarez", ResolvedAt=DateTime.UtcNow.AddDays(-20) },
                new Bug { BugCode="BUG-217", Title="Document permissions not visible in review", Description="Client reviewers need a clear permissions summary.", ProjectId=projects[2].Id, TaskItemId=tasks[3].Id, Severity=BugSeverity.Critical, Priority=Priority.Critical, Status=BugStatus.Open, Reporter="Leila Patel" }
            };
            context.Bugs.AddRange(bugs);
            await context.SaveChangesAsync();

            var releases = new[]
            {
                new Release { ReleaseCode="REL-24", Version="v2.4.0", Name="CareConnect reminders", ProjectId=projects[0].Id, ReleaseDate=DateTime.UtcNow.AddDays(18), Status=ReleaseStatus.Testing, Notes="Includes reminder preferences and timeline grouping." },
                new Release { ReleaseCode="REL-23", Version="v1.8.0", Name="Route exception workflows", ProjectId=projects[1].Id, ReleaseDate=DateTime.UtcNow.AddDays(30), Status=ReleaseStatus.InDevelopment, Notes="Dispatch workflow and driver exception handling." },
                new Release { ReleaseCode="REL-22", Version="v1.0.0", Name="Factory Pulse launch", ProjectId=projects[3].Id, ReleaseDate=DateTime.UtcNow.AddDays(-28), Status=ReleaseStatus.Released, Notes="First production visibility release." }
            };
            context.Releases.AddRange(releases);

            context.Communications.AddRange(
                new Communication { ClientId=clients[0].Id, ProjectId=projects[0].Id, Type="Review Meeting", Subject="CareConnect sprint review", Notes="Maya confirmed reminder cadence and escalation language.", Date=DateTime.UtcNow.AddDays(-1) },
                new Communication { ClientId=clients[1].Id, ProjectId=projects[1].Id, Type="Requirement Discussion", Subject="Exception workflow discovery", Notes="Captured dispatcher constraints and depot handoff rules.", Date=DateTime.UtcNow.AddDays(-3) },
                new Communication { ClientId=clients[2].Id, ProjectId=projects[2].Id, Type="Email", Subject="Security review follow-up", Notes="Client provided permission matrix feedback.", Date=DateTime.UtcNow.AddDays(-5) }
            );
            context.ActivityLogs.AddRange(
                new ActivityLog { UserName="Olivia Morgan", Action="Approved", Entity="Requirement", EntityId="REQ-419", Description="Approved single patient timeline analysis.", Timestamp=DateTime.UtcNow.AddHours(-2) },
                new ActivityLog { UserName="Marcus Lee", Action="Assigned", Entity="Task", EntityId="TASK-768", Description="Assigned route constraint rules to Daniel Kim.", Timestamp=DateTime.UtcNow.AddHours(-6) },
                new ActivityLog { UserName="Sofia Alvarez", Action="Created", Entity="Bug", EntityId="BUG-217", Description="Reported document permission visibility issue.", Timestamp=DateTime.UtcNow.AddDays(-1) },
                new ActivityLog { UserName="Nikhil Shah", Action="Published", Entity="Release", EntityId="REL-22", Description="Published Factory Pulse v1.0.0.", Timestamp=DateTime.UtcNow.AddDays(-28) }
            );
            context.Notifications.AddRange(
                new Notification { UserId="demo", Title="Requirement approved", Message="REQ-419 is ready for delivery planning.", CreatedAt=DateTime.UtcNow.AddHours(-2) },
                new Notification { UserId="demo", Title="Critical bug reported", Message="BUG-217 needs triage before the next review.", CreatedAt=DateTime.UtcNow.AddDays(-1) },
                new Notification { UserId="demo", Title="Sprint checkpoint", Message="Sprint 31 is 64% complete with 6 days remaining.", CreatedAt=DateTime.UtcNow.AddDays(-1) }
            );

            await context.SaveChangesAsync();
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Administrator", "Project Manager", "Developer", "Tester", "Client" })
            if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
        if (await userManager.FindByEmailAsync("olivia.morgan@devtrack.local") is null)
        {
            var user = new ApplicationUser { UserName="olivia.morgan@devtrack.local", Email="olivia.morgan@devtrack.local", DisplayName="Olivia Morgan", RoleLabel="Project Manager", EmailConfirmed=true };
            await userManager.CreateAsync(user, "DevTrack123");
            await userManager.AddToRoleAsync(user, "Project Manager");
        }
    }
}
