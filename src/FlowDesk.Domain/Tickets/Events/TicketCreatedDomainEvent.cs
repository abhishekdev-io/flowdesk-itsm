using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.ValueObjects;

namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketCreatedDomainEvent(
    Guid WorkspaceId,
    Guid TicketId,
    Guid CreatedBy,
    TicketTitle Title,
    TicketDescription Description,
    TicketPriority Priority,
    TicketType Type
) : IDomainEvent 
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
