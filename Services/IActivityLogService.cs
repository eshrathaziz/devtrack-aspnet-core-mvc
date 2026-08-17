namespace DevTrack.Services;

public interface IActivityLogService
{
    Task RecordAsync(string userName, string action, string entityName, string? entityId, string description);
}
