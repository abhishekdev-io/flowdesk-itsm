using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketTypeChangedActivityLogHandler(
    ITicketActivityService ticketActivity) 
    : INotificationHandler<TicketTypeChangedDomainEvent>
{
    public async Task Handle(TicketTypeChangedDomainEvent notification, CancellationToken cancellationToken)
    {        
        var description = $"Ticket type was changed from {notification.OldType} to {notification.NewType}.";

        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.ChangedBy,
            notification.OccurredOn,
            TicketActivityType.TypeChanged,
            description,
            cancellationToken
        );
    }
}
