using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketAssignActivityLogHandler(
    ITicketActivityService ticketActivity) 
    : INotificationHandler<TicketAssignDomainEvent>
{
    public async Task Handle(TicketAssignDomainEvent notification, CancellationToken cancellationToken)
    {
        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.PerformedBy,
            notification.OccurredOn,
            TicketActivityType.Assigned,
            $"Ticket was assigned to a new agent and status changed from {notification.OldStatus} to {notification.NewStatus}.",
            cancellationToken
        );
    }
}
