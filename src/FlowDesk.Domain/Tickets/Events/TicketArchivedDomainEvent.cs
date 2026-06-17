namespace FlowDesk.Domain.Tickets.Events;

public sealed record TicketArchivedDomainEvent(
    Guid TicketId,
    Guid PerformedBy,
    bool IsArchived 
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
