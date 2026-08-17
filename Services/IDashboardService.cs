using DevTrack.ViewModels;

namespace DevTrack.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(string? userId, bool clientOnly);
}
