using DevTrack.Data;
using DevTrack.Models;

namespace DevTrack.Services;

public class ActivityLogService(ApplicationDbContext context) : IActivityLogService
{
    public async Task RecordAsync(string userName, string action, string entityName, string? entityId, string description)
    {
        context.ActivityLogs.Add(new ActivityLog { UserName = userName, Action = action, EntityName = entityName, EntityId = entityId, Description = description });
        await context.SaveChangesAsync();
    }
}
