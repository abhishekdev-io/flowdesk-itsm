using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketPriorityChangedDomainEvent(
    Guid WorkspaceId,
    Guid TicketCreatedBy,
    Guid TicketId,
    Guid ChangedBy,
    string TicketNumber,
    TicketPriority OldPriority,
    TicketPriority NewPriority
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
