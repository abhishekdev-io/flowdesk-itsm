namespace FlowDesk.Domain.Tickets.Enums;

public enum TicketType
{
    Incident = 1, //something is broken
    ServiceRequest = 2, // i need something new
    Problem = 3, // rootcause created by admin or agent if something is root of multiple incidents
    ChangeRequest = 4
}
