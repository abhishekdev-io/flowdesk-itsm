namespace FlowDesk.Domain.Tickets.Enums;

public enum TicketActivityType
{
    // Lifecycle
    Created = 1,
    StatusChanged = 2,
    Archived = 3,
    UnArchived = 4,
    Assigned = 5,
    Unassigned = 6,
    Updated = 7,            
    PriorityChanged = 8,
    TypeChanged = 9,
    SlaBreached = 10,      
    CommentAdded = 11

}
