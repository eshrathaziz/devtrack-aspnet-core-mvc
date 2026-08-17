namespace DevTrack.Models;

public enum ClientStatus { Active, Prospect, Inactive }
public enum ProjectStatus { Planning, Active, OnHold, Completed, Cancelled }
public enum Priority { Low, Medium, High, Critical }
public enum RequirementType { Functional, NonFunctional, Business, Technical, ChangeRequest }
public enum RequirementStatus { New, UnderAnalysis, ClientReview, Approved, Rejected, InDevelopment, Completed }
public enum SprintStatus { Planned, Active, Completed }
public enum TaskStatus { Backlog, ToDo, InProgress, CodeReview, Testing, Done, Blocked }
public enum ExperienceLevel { Junior, Mid, Senior, Lead }
public enum AvailabilityStatus { Available, Limited, FullyAllocated, OnLeave }
public enum BugSeverity { Low, Medium, High, Critical }
public enum BugStatus { Open, Assigned, InProgress, Fixed, Testing, Closed, Reopened }
public enum ReleaseStatus { Planned, InDevelopment, Testing, Released, Cancelled }
public enum AnalysisDecision { Draft, Approved, Rejected }
public enum CommunicationType { Email, PhoneCall, Meeting, RequirementDiscussion, ChangeRequest, ReviewMeeting, FollowUp }
public enum NotificationType { TaskAssigned, RequirementApproved, RequirementRejected, BugAssigned, SprintStarted, SprintCompleted, ClientComment, ProjectMilestone, ReleasePublished }
