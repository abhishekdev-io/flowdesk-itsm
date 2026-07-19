using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.Tickets.ValueObjects;

public sealed class TicketDescription : ValueObject
{
    private const int MaxLength = 500;
    public string Value { get; }

    private TicketDescription(string value)
    => Value = value;

    public static TicketDescription Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        throw new DomainException("Ticket description cannot be empty.");

        string trimmedValue = value.Trim();

        if (trimmedValue.Length > MaxLength)
            throw new DomainException("Ticket description cannot exceed 500 characters.");

        return new TicketDescription(trimmedValue);
    }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
 