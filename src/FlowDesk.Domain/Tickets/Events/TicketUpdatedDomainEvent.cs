using FlowDesk.Domain.Tickets.ValueObjects;

namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketUpdatedDomainEvent(
    Guid TicketId,
    Guid UpdatedBy,
    TicketTitle OldTitle,
    TicketTitle NewTitle,
    TicketDescription OldDescription,
    TicketDescription NewDescription
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
