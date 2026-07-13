using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Domain.Tickets.Aggregates;

public sealed class TicketActivity : Entity<Guid>
{
    public Guid TicketId { get; private set; }
    public Guid PerformedBy { get; private set; }
    public TicketActivityType Action { get; private set; } 
    public string? Description { get; private set; }
    public DateTime OccurredAt { get; private set; }

    public Ticket Ticket { get; private set; } = null!;

    private TicketActivity() : base(Guid.Empty) { } 

    public static TicketActivity Create(
        Guid ticketId,
        Guid performedBy,
        DateTime occurredAt, 
        TicketActivityType action, 
        string? description = null)
    {
        if (ticketId == Guid.Empty)
            throw new DomainException($"Invalid ticket Id-{ticketId}.");

        if (performedBy == Guid.Empty)
            throw new DomainException($"Invalid user Id-{performedBy}.");

        return new TicketActivity
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            PerformedBy = performedBy,
            Action = action,
            Description = description,
            OccurredAt = occurredAt
        };
    }
}
