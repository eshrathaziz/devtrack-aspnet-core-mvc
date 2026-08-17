using DevTrack.Models;
using TaskStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Services;

public interface IWorkflowService
{
    bool CanTransition(TaskStatus from, TaskStatus to);
    bool CanTransition(BugStatus from, BugStatus to);
}
