using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.Tickets.ValueObjects;

public sealed class TicketTitle : ValueObject
{
    private const int MaxLength = 100;  
    public string Value { get;}

    
    private TicketTitle(string value)
        => Value = value;
        
    public static TicketTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Ticket title cannot be empty.");

        string trimmedValue = value.Trim();    

        if (trimmedValue.Length > MaxLength)
            throw new DomainException("Ticket title cannot exceed 100 characters.");

        return new TicketTitle(trimmedValue);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

