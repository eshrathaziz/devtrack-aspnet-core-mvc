using DevTrack.Models;
using TaskStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Services;

public class WorkflowService : IWorkflowService
{
    public bool CanTransition(TaskStatus from, TaskStatus to) => from switch
    {
        TaskStatus.Backlog => to is TaskStatus.ToDo or TaskStatus.Blocked,
        TaskStatus.ToDo => to is TaskStatus.InProgress or TaskStatus.Blocked or TaskStatus.Backlog,
        TaskStatus.InProgress => to is TaskStatus.CodeReview or TaskStatus.Testing or TaskStatus.Blocked or TaskStatus.ToDo,
        TaskStatus.CodeReview => to is TaskStatus.InProgress or TaskStatus.Testing or TaskStatus.Blocked,
        TaskStatus.Testing => to is TaskStatus.Done or TaskStatus.InProgress or TaskStatus.Blocked,
        TaskStatus.Blocked => to is TaskStatus.ToDo or TaskStatus.InProgress,
        TaskStatus.Done => false,
        _ => false
    };

    public bool CanTransition(BugStatus from, BugStatus to) => from switch
    {
        BugStatus.Open => to is BugStatus.Assigned or BugStatus.Closed,
        BugStatus.Assigned => to is BugStatus.InProgress or BugStatus.Open,
        BugStatus.InProgress => to is BugStatus.Fixed or BugStatus.Reopened,
        BugStatus.Fixed => to is BugStatus.Testing or BugStatus.Reopened,
        BugStatus.Testing => to is BugStatus.Closed or BugStatus.Reopened,
        BugStatus.Reopened => to is BugStatus.Assigned or BugStatus.InProgress,
        BugStatus.Closed => false,
        _ => false
    };
}
