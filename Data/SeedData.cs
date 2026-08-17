using DevTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Data;

public static class SeedData
{
    private static readonly string[] Roles = ["Administrator", "Project Manager", "Developer", "Tester", "Client"];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (await context.Clients.AnyAsync()) return;

        var password = scope.ServiceProvider.GetRequiredService<IConfiguration>()["DemoSeed:Password"]
            ?? throw new InvalidOperationException("Set DemoSeed:Password with user secrets before enabling SeedDemoData.");
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles) if (!await roles.RoleExistsAsync(role)) await roles.CreateAsync(new IdentityRole(role));

        var admin = await EnsureUserAsync(users, "admin@devtrack.local", "Ada Morgan", password, "Administrator");
        var manager = await EnsureUserAsync(users, "manager@devtrack.local", "Maya Chen", password, "Project Manager");
        var developerUser = await EnsureUserAsync(users, "developer@devtrack.local", "Noah Williams", password, "Developer");
        var tester = await EnsureUserAsync(users, "tester@devtrack.local", "Priya Shah", password, "Tester");
        var clientUser = await EnsureUserAsync(users, "client@devtrack.local", "Jordan Bell", password, "Client");

        var clientSpecs = new (string Name, string Contact, string Email, string Industry, string City)[]
        {
            ("Apex Health Systems", "Jordan Bell", "jordan.bell@apexhealth.example", "Healthcare", "Boston"),
            ("NovaCore Logistics", "Dina Foster", "dina.foster@novacore.example", "Logistics", "Chicago"),
            ("Vertex Financial Services", "Marcus Reed", "marcus.reed@vertexfs.example", "Financial Services", "New York"),
            ("BlueOrbit Manufacturing", "Elena Park", "elena.park@blueorbit.example", "Manufacturing", "Detroit"),
            ("GreenField Retail", "Samir Khan", "samir.khan@greenfield.example", "Retail", "Austin"),
            ("Harborline Energy", "Rose Delgado", "rose.delgado@harborline.example", "Energy", "Houston"),
            ("CedarWorks Education", "Leo Wong", "leo.wong@cedarworks.example", "Education", "Seattle"),
            ("Northstar Mobility", "Claire Bennett", "claire.bennett@northstar.example", "Mobility", "Denver")
        };
        var clients = clientSpecs.Select((x, i) => new Client { CompanyName = x.Name, ContactPerson = x.Contact, Email = x.Email, Industry = x.Industry, City = x.City, Country = "United States", Phone = $"+1 555 01{i:00}", AccountManager = manager.DisplayName, ApplicationUserId = i == 0 ? clientUser.Id : null }).ToList();
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();

        var projectSpecs = new (string Name, int ClientIndex, string Description, ProjectStatus Status, Priority Priority, int Progress, int EndOffset)[]
        {
            ("CarePath Patient Portal", 0, "Secure patient scheduling and care coordination portal.", ProjectStatus.Active, Priority.High, 68, 55),
            ("RoutePulse Operations Hub", 1, "Dispatch visibility and delivery exception workflow.", ProjectStatus.Active, Priority.Critical, 43, 72),
            ("Client Onboarding Console", 2, "Regulated client onboarding and compliance review.", ProjectStatus.Active, Priority.High, 31, 100),
            ("Plant Signals Upgrade", 3, "Manufacturing alerting and equipment-status interface.", ProjectStatus.Completed, Priority.Medium, 100, -12),
            ("Storefront Stock Insight", 4, "Near-real-time store inventory analysis.", ProjectStatus.Planning, Priority.Medium, 8, 130),
            ("Grid Service Desk", 5, "Incident intake and field-service dispatch product.", ProjectStatus.OnHold, Priority.Low, 18, 145)
        };
        var projects = projectSpecs.Select((x, i) => new Project { Name = x.Name, ClientId = clients[x.ClientIndex].Id, Description = x.Description, ProjectManager = manager.DisplayName, StartDate = DateTime.UtcNow.Date.AddDays(-90 + i * 12), EndDate = DateTime.UtcNow.Date.AddDays(x.EndOffset), Status = x.Status, Priority = x.Priority, Progress = x.Progress }).ToList();
        context.Projects.AddRange(projects);
        var developers = new List<DeveloperProfile>
        {
            new() { ApplicationUserId = developerUser.Id, DisplayName = developerUser.DisplayName!, Skills = "C#, ASP.NET Core, SQL Server, JavaScript", ExperienceLevel = ExperienceLevel.Junior, CurrentWorkload = 72, Availability = AvailabilityStatus.Limited },
            new() { DisplayName = "Iris Patel", Skills = "C#, Azure, REST APIs, Testing", ExperienceLevel = ExperienceLevel.Mid, CurrentWorkload = 54, Availability = AvailabilityStatus.Available },
            new() { DisplayName = "Owen Scott", Skills = "C#, Entity Framework Core, Reporting", ExperienceLevel = ExperienceLevel.Senior, CurrentWorkload = 84, Availability = AvailabilityStatus.Limited }
        };
        context.DeveloperProfiles.AddRange(developers);
        await context.SaveChangesAsync();

        var requirementTitles = new[] { "Appointment scheduling workflow", "Clinical document access controls", "Delivery exception triage", "Depot workload view", "KYC review handoff", "Evidence retention policy", "Plant alert acknowledgement", "Maintenance exception notes", "Inventory alert thresholds", "Store replenishment forecast", "Field incident intake", "Dispatch escalation rules", "Learner access roles", "Route ETA quality rules", "Release audit export" };
        var requirements = requirementTitles.Select((title, i) => new Requirement { ClientId = projects[i % projects.Count].ClientId, ProjectId = projects[i % projects.Count].Id, Title = title, Description = $"Business requirement and acceptance details for {title.ToLowerInvariant()}.", Type = (RequirementType)(i % 5), Priority = i % 6 == 0 ? Priority.Critical : i % 3 == 0 ? Priority.High : Priority.Medium, Status = (RequirementStatus)(i % 7), BusinessValue = 5 + (i % 6), AssignedManager = manager.DisplayName, DueDate = DateTime.UtcNow.Date.AddDays(10 + i * 3) }).ToList();
        context.Requirements.AddRange(requirements);
        var sprints = new List<Sprint>();
        for (var i = 0; i < 8; i++) sprints.Add(new Sprint { ProjectId = projects[i % 3].Id, Name = $"{projects[i % 3].Name.Split(' ')[0]}-{i + 1:00}", Goal = $"Deliver the next verified increment for {projects[i % 3].Name}.", StartDate = DateTime.UtcNow.Date.AddDays(-7 + i * 14), EndDate = DateTime.UtcNow.Date.AddDays(6 + i * 14), Status = i == 0 || i == 2 ? SprintStatus.Active : i < 2 ? SprintStatus.Completed : SprintStatus.Planned });
        context.Sprints.AddRange(sprints);
        await context.SaveChangesAsync();

        var taskTitles = new[] { "Map provider availability rules", "Validate appointment state changes", "Add patient reminder service", "Review care-team access", "Build exception assignment queue", "Write dispatch acceptance cases", "Capture depot override decisions", "Model KYC reviewer decision", "Build evidence checklist", "Validate document audit trail", "Create plant alert inbox", "Record maintenance acknowledgement", "Define inventory threshold rules", "Review replenishment projections", "Implement incident triage", "Add field escalation contact", "Capture learner permissions", "Verify journey accessibility", "Profile route ETA calculation", "Add delivery exception notes", "Prepare release deployment plan", "Document rollback criteria", "Review client feedback log", "Refine sprint goal", "Test critical notification path", "Resolve authorization defect", "Validate project progress rollup", "Write technical handover notes", "Prepare release notes", "Close completed work items" };
        var statuses = new[] { TaskStatus.InProgress, TaskStatus.CodeReview, TaskStatus.ToDo, TaskStatus.Backlog, TaskStatus.Testing, TaskStatus.Done, TaskStatus.Blocked };
        var tasks = taskTitles.Select((title, i) =>
        {
            var requirement = requirements[i % requirements.Count];
            var projectId = requirement.ProjectId!.Value;
            return new TaskItem { ProjectId = projectId, SprintId = sprints.First(s => s.ProjectId == projectId).Id, RequirementId = requirement.Id, DeveloperProfileId = i % 5 == 0 ? null : developers[i % developers.Count].Id, Title = title, Description = $"Implementation work for {title.ToLowerInvariant()}.", Priority = i % 8 == 0 ? Priority.Critical : i % 3 == 0 ? Priority.High : Priority.Medium, Status = statuses[i % statuses.Length], StoryPoints = 2 + (i % 8), EstimatedHours = 4 + (i % 5) * 4, ActualHours = i % 7 == 5 ? 8 : 0, DueDate = DateTime.UtcNow.Date.AddDays(2 + i) };
        }).ToList();
        context.TaskItems.AddRange(tasks);
        await context.SaveChangesAsync();

        var bugTitles = new[] { "Timezone change shows unavailable slot", "SLA override has no fallback owner", "KYC evidence refresh is skipped", "Plant alert acknowledgement repeats", "Threshold update is not visible", "Incident attachments lose ordering", "Access role is not refreshed", "Route ETA refresh delays response", "Release notes omit resolved bug", "Task workload percentage rounds incorrectly", "Client timeline sorts follow-up incorrectly", "Sprint completion counts blocked item", "Critical notification does not link to task", "Audit history omits status transition", "Requirement approval does not notify client" };
        context.Bugs.AddRange(bugTitles.Select((title, i) => new Bug { ProjectId = tasks[i].ProjectId, TaskItemId = tasks[i].Id, Title = title, Description = $"Reproducible defect: {title.ToLowerInvariant()}.", Severity = (BugSeverity)(i % 4), Priority = i % 4 == 3 ? Priority.Critical : Priority.High, AssignedDeveloperId = developers[i % developers.Count].Id, Reporter = tester.DisplayName, Status = (BugStatus)(i % 7) }));
        context.Releases.AddRange(Enumerable.Range(0, 5).Select(i => new Release { ProjectId = projects[i].Id, Version = $"{2 + i}.0.0", Name = $"{projects[i].Name} milestone", ReleaseDate = DateTime.UtcNow.Date.AddDays(21 + i * 14), Status = (ReleaseStatus)(i % 4), Notes = "Release scope, deployment notes, and known constraints are recorded here." }));
        context.ClientCommunications.AddRange(Enumerable.Range(0, 8).Select(i => new ClientCommunication { ClientId = clients[i].Id, ProjectId = projects[i % projects.Count].Id, Type = (CommunicationType)(i % 7), Subject = $"Delivery coordination: {projects[i % projects.Count].Name}", Description = "Client coordination record for confirmed decisions, risks, and follow-up actions.", Participants = $"{clients[i].ContactPerson}, {manager.DisplayName}", Employee = manager.DisplayName, OccurredAtUtc = DateTime.UtcNow.AddDays(-i - 1), FollowUpDateUtc = DateTime.UtcNow.AddDays(i + 4) }));
        context.ActivityLogs.AddRange(Enumerable.Range(0, 12).Select(i => new ActivityLog { UserName = i % 2 == 0 ? manager.DisplayName! : developerUser.DisplayName!, Action = i % 3 == 0 ? "Requirement Approved" : i % 3 == 1 ? "Task Status Updated" : "Bug Created", EntityName = i % 3 == 2 ? "Bug" : i % 3 == 1 ? "Task" : "Requirement", EntityId = (i + 1).ToString(), Description = "Recorded delivery activity for the active programme.", CreatedAtUtc = DateTime.UtcNow.AddHours(-i * 3) }));
        context.Notifications.AddRange(Enumerable.Range(0, 5).Select(i => new Notification { ApplicationUserId = admin.Id, Type = (NotificationType)(i % 5), Title = "Delivery update recorded", Message = "A new item requires review in the DevTrack workspace.", Link = "/Dashboard", CreatedAtUtc = DateTime.UtcNow.AddHours(-i) }));
        await context.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> EnsureUserAsync(UserManager<ApplicationUser> users, string email, string name, string password, string role)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, DisplayName = name };
            var result = await users.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        if (!await users.IsInRoleAsync(user, role)) await users.AddToRoleAsync(user, role);
        return user;
    }
}
