using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.Tickets.Aggregates;
 
public sealed class TicketComment : Entity<Guid>
{
    public Guid TicketId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsEdited { get; private set; }
    public bool IsDeleted { get; private set; } 
    public DateTime? DeletedAt { get; private set; }
    public DateTime? RestoredAt { get; private set; }   

    public Ticket Ticket { get; private set; } = null!;

    private TicketComment() : base(Guid.Empty) { }

    //Create
    public static TicketComment Create(
        Guid ticketId,
        string description,
        Guid createdBy)
    {
        if (ticketId == Guid.Empty)
            throw new DomainException("Invalid ticket Id.");

        if (createdBy == Guid.Empty)
            throw new DomainException("Invalid user Id.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Comment description cannot be empty.");

        return new TicketComment
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            EditedAt = null
        };
    }

    //Update
    public void Edit(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Comment description cannot be empty.");

        Description = description;
        EditedAt = DateTime.UtcNow;
        IsEdited = true;
    }
     
    //Delete
    public void Delete()
    {
        if (IsDeleted)
            throw new DomainException("Comment is already deleted");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    //Restore
    public void Restore()
    {
        if (!IsDeleted)
            throw new DomainException("Only deleted comment can be restored.");

        IsDeleted = false;
        RestoredAt = DateTime.UtcNow;
    }
}
