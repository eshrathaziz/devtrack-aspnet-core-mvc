# DevTrack Interview Guide

This guide is intentionally limited to code and configuration that exist in the DevTrack repository.

| Question | Evidence-based answer |
| --- | --- |
| Why did you build DevTrack? | It demonstrates a software team’s real delivery chain: client requirement, analysis, project, sprint, task, developer assignment, testing, defect management, and release preparation. |
| How is MVC used? | Controllers in `Controllers/` receive requests and return Razor views in `Views/`. Models represent domain data; view models such as `DashboardViewModel` and `ReportsViewModel` shape read data for a screen. |
| Where is dependency injection used? | `Program.cs` registers `ApplicationDbContext`, `DashboardService`, `ActivityLogService`, and `WorkflowService`. Controllers declare the services they need in their constructors. |
| How does the dashboard get its numbers? | `DashboardService` uses asynchronous EF Core/LINQ queries against `ApplicationDbContext`; it groups project, task, and bug data before creating `DashboardViewModel`. It does not hard-code metrics. |
| How is SQL Server configured? | `Program.cs` configures `UseSqlServer` with `ConnectionStrings:DefaultConnection`. `DevTrack.csproj` references `Microsoft.EntityFrameworkCore.SqlServer`, and `Migrations/` contains the generated schema migration. |
| How did you model relationships? | `ApplicationDbContext.OnModelCreating` defines navigation relationships, delete behaviours, unique indexes for client names/project names/release versions, and one-to-one mappings from `ApplicationUser` to client and developer profiles. |
| Why use `TaskItem` rather than `Task`? | `Task` is a framework type used for asynchronous work. Naming the entity `TaskItem` avoids a collision while keeping the business language clear. |
| How are status transitions protected? | `WorkflowService` allows only defined next states for task and bug workflows. `TasksController.UpdateStatus` and `BugsController.UpdateStatus` reject invalid transitions and provide friendly feedback. |
| How do requirements become development work? | A requirement is stored with client/project context and a manager can record analysis. The requirement details view exposes linked task creation; `TaskItem.RequirementId` creates the relationship. |
| How is authorization implemented? | Identity roles are provisioned in `SeedData`. Controllers use `[Authorize]` and role restrictions such as `Administrator,Project Manager` for management actions, `Tester` for defect creation, and `Client` for requirement submission. |
| How do you prevent a client seeing another client’s data? | `ProjectsController` and `RequirementsController` filter queryable data by `Client.ApplicationUserId == currentUserId`; `DashboardService` applies the same scope for client dashboards. |
| Where is LINQ used? | LINQ is used for filters, ordering, groups, counts, and projections in the dashboard, reporting, and CRUD controller actions. The queries execute asynchronously through EF Core. |
| Where does jQuery appear? | `wwwroot/js/site.js` handles the mobile sidebar, local table filtering, and anti-forgery-protected AJAX comment posts. Razor forms include jQuery Validation via `_ValidationScriptsPartial`. |
| How is audit history implemented? | Controllers call `IActivityLogService.RecordAsync` after operational events. The service writes `ActivityLog` records that the dashboard and report views display. |
| What can you verify from the repository? | The solution builds with no warnings/errors and the xUnit workflow suite contains 19 passing tests. Migration script generation was verified without a live SQL Server connection. A database update and full authenticated browser flow require SQL Server credentials and an instance. |
