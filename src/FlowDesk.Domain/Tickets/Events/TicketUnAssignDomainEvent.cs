using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.ValueObjects;

namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketUnAssignDomainEvent(
    Guid WorkspaceId,
    Guid TicketId,
    Guid PerformedBy,
    Guid? WasAssignedTo,
    TicketTitle Title,
    TicketStatus OldStatus,
    TicketStatus NewStatus
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
 