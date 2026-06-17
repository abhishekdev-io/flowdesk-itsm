using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketTypeChangedDomainEvent(
    Guid TicketId,
    Guid ChangedBy,
    TicketType OldType,
    TicketType NewType
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
