# DevTrack Interview Guide

This guide maps interview topics to the implementation that exists in the repository.

## C# and application design

**Where is object-oriented design used?** DevTrack models represent business entities such as `Project`, `Requirement`, `Sprint`, `TaskItem`, `Bug`, and `Release`. Services isolate dashboard aggregation and activity recording from controllers. Records are used for immutable dashboard projections such as `ProjectSummary` and `SprintSummary`.

**Where is async/await used?** Controllers and services use async EF Core methods such as `ToListAsync`, `CountAsync`, `FindAsync`, and `SaveChangesAsync`. Startup seeding is asynchronous as well.

**How is exception handling approached?** Production environments route unhandled exceptions to `/Home/Error` rather than displaying technical details. Model validation and friendly `NotFound` responses are used at controller boundaries.

## ASP.NET Core MVC

**Explain the MVC architecture.** Controllers receive HTTP requests and coordinate data access or services. Models describe the domain and validation rules. Razor Views render the user experience. The default route points to `DashboardController.Index`, while module controllers expose focused workflows such as `ProjectsController`, `RequirementsController`, `TasksController`, and `BugsController`.

**Where is dependency injection used?** `ApplicationDbContext`, `DashboardService`, and `ActivityService` are registered in `Program.cs` and injected into controllers through primary constructors.

**How does model binding and validation work?** Form fields bind to model properties such as `Project`. Required, email, and maximum-length data annotations define server-side validation rules. State-changing forms include anti-forgery tokens.

**How is authorization started?** ASP.NET Core Identity is registered with role support and the roles Administrator, Project Manager, Developer, Tester, and Client are seeded. The next hardening step is to add `[Authorize(Roles = "...")]` policies to every module action.

## Entity Framework Core

**What does `ApplicationDbContext` do?** It inherits from `IdentityDbContext<ApplicationUser>`, exposes DbSets for the DevTrack entities, defines unique indexes, and configures relationships and delete behavior.

**Where is LINQ used?** Dashboard counts, grouped status dictionaries, filtered module lists, project health summaries, sprint progress, and quality queues are all produced with LINQ queries against EF Core.

**How does the requirement workflow connect to delivery?** A requirement belongs to a client and project, can have one `RequirementAnalysis`, and can be associated with tasks. Requirement status represents the journey from New and Under Analysis through Approval and In Development.

**How does the task workflow protect status quality?** `TasksController.UpdateStatus` defines an explicit transition map. For example, a task in `InProgress` can move to `CodeReview` or `Blocked`, while a completed task has no outgoing transitions.

## SQL Server and relational design

**How is SQL Server configured?** `appsettings.json` contains a `DefaultConnection` placeholder and `Program.cs` selects `UseSqlServer` when `Database:Provider` is `sqlserver`. SQLite is selected by default only for the self-contained local preview.

**Why are indexes used?** Business identifiers such as project, requirement, task, bug, and release codes are unique indexed values. This prevents duplicates and keeps lookups efficient.

## Frontend implementation

**Where is Razor used?** All primary screens are `.cshtml` Razor Views under `Views/Dashboard`, `Views/Projects`, `Views/Requirements`, `Views/Tasks`, `Views/Bugs`, `Views/Clients`, `Views/Developers`, and `Views/Releases`.

**Where is jQuery used?** `wwwroot/js/site.js` handles the mobile navigation, global search shortcut, search-form feedback, and status-select submission. jQuery is loaded from a CDN in the shared layout.

**How does the dashboard chart work?** The controller prepares grouped task-status data from EF Core. The Razor view serializes that dictionary into a Chart.js bar chart without hard-coding the counts.

## Project questions

**Why build DevTrack?** Software teams need a shared view of client intent, delivery work, quality risks, and release readiness. DevTrack demonstrates that workflow in a realistic internal application rather than a disconnected CRUD sample.

**How would you scale the application?** Add pagination and projection-based queries to high-volume lists, move reporting to read models, introduce background notification processing, add integration tests, and deploy SQL Server with managed identity and structured logging.

**What is the strongest portfolio signal?** The project demonstrates a connected workflow: requirements become tasks, tasks belong to sprints and developers, bugs point back to tasks and projects, releases represent shipped outcomes, and activity logs make changes visible.
