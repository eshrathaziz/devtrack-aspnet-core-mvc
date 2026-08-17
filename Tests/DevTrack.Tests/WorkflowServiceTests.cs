using DevTrack.Models;
using DevTrack.Services;
using TaskStatus = DevTrack.Models.TaskStatus;

namespace DevTrack.Tests;

public class WorkflowServiceTests
{
    private readonly WorkflowService _service = new();

    [Theory]
    [InlineData(TaskStatus.Backlog, TaskStatus.ToDo)]
    [InlineData(TaskStatus.ToDo, TaskStatus.InProgress)]
    [InlineData(TaskStatus.InProgress, TaskStatus.CodeReview)]
    [InlineData(TaskStatus.CodeReview, TaskStatus.Testing)]
    [InlineData(TaskStatus.Testing, TaskStatus.Done)]
    [InlineData(TaskStatus.Blocked, TaskStatus.InProgress)]
    public void Task_status_transition_is_allowed_when_it_follows_the_delivery_flow(TaskStatus from, TaskStatus to)
    {
        Assert.True(_service.CanTransition(from, to));
    }

    [Theory]
    [InlineData(TaskStatus.Backlog, TaskStatus.Done)]
    [InlineData(TaskStatus.ToDo, TaskStatus.Testing)]
    [InlineData(TaskStatus.Done, TaskStatus.InProgress)]
    [InlineData(TaskStatus.Testing, TaskStatus.CodeReview)]
    public void Task_status_transition_is_rejected_when_it_skips_required_delivery_stages(TaskStatus from, TaskStatus to)
    {
        Assert.False(_service.CanTransition(from, to));
    }

    [Theory]
    [InlineData(BugStatus.Open, BugStatus.Assigned)]
    [InlineData(BugStatus.Assigned, BugStatus.InProgress)]
    [InlineData(BugStatus.InProgress, BugStatus.Fixed)]
    [InlineData(BugStatus.Fixed, BugStatus.Testing)]
    [InlineData(BugStatus.Testing, BugStatus.Closed)]
    [InlineData(BugStatus.Reopened, BugStatus.InProgress)]
    public void Bug_status_transition_is_allowed_when_it_follows_the_quality_workflow(BugStatus from, BugStatus to)
    {
        Assert.True(_service.CanTransition(from, to));
    }

    [Theory]
    [InlineData(BugStatus.Open, BugStatus.Fixed)]
    [InlineData(BugStatus.Assigned, BugStatus.Closed)]
    [InlineData(BugStatus.Closed, BugStatus.Reopened)]
    public void Bug_status_transition_is_rejected_when_it_breaks_the_quality_workflow(BugStatus from, BugStatus to)
    {
        Assert.False(_service.CanTransition(from, to));
    }
}
