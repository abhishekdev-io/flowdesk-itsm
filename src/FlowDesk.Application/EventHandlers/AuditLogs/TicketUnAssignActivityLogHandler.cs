using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketUnAssignActivityLogHandler(
    ITicketActivityService ticketActivity) 
    : INotificationHandler<TicketUnAssignDomainEvent>
{
    public async Task Handle(TicketUnAssignDomainEvent notification, CancellationToken cancellationToken)
    {        
        var  description = $"Ticket was unassigned from the agent and status changed from {notification.OldStatus} to {notification.NewStatus}.";
        
        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.PerformedBy,
            notification.OccurredOn,
            TicketActivityType.Unassigned,
            description,
            cancellationToken
        );

    }
}

