using MediatR;

namespace FlowDesk.Domain.Primitives;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}

