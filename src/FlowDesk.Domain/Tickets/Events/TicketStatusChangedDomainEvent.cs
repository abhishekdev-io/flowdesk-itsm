using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketStatusChangedDomainEvent(
    Guid WorkspaceId,
    Guid TicketCreatedBy,
    Guid TicketId,
    Guid ChangedBy,
    string TicketNumber,
    TicketStatus OldStatus,
    TicketStatus NewStatus,
    string? Reason = null
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
