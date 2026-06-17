namespace FlowDesk.Domain.Notifications;

public enum NotificationEventType
{
    TicketCreated = 1,
    TicketAssigned = 2,
    TicketUnassigned = 3,
    TicketStatusChanged = 4,
    TicketCommentAdded = 5,
    TicketPriorityChanged = 6,
    AddedToWorkspace = 7,
    RemovedFromWorkspace = 8,
    WorkspaceRoleChanged = 9
}