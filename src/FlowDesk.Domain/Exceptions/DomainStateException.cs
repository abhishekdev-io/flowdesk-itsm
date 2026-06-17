namespace FlowDesk.Domain.Exceptions;

public class DomainStateException(string message)
    : DomainException(message)
{
}
