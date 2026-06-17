using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketPriorityActivityLogHandler(
    ITicketActivityService ticketActivity
)   : INotificationHandler<TicketPriorityChangedDomainEvent>
{
    public async Task Handle(TicketPriorityChangedDomainEvent notification, CancellationToken cancellationToken)
    {        
        var description = $"Ticket priority was changed from {notification.OldPriority} to {notification.NewPriority}.";

        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.ChangedBy,
            notification.OccurredOn,
            TicketActivityType.PriorityChanged,
            description,
            cancellationToken
        );
    }
}
