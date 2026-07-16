using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketArchivedActivityLogHandler(
    ITicketActivityService ticketActivity)
    : INotificationHandler<TicketArchivedDomainEvent>
{
    public async Task Handle(TicketArchivedDomainEvent notification, CancellationToken cancellationToken)
    {
        string description = notification.IsArchived 
            ? "Ticket was successfully archived and removed from active work queues." 
            : "Ticket was unarchived and restored.";

        TicketActivityType actionType = notification.IsArchived 
            ? TicketActivityType.Archived 
            : TicketActivityType.UnArchived;
        
        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.PerformedBy,
            notification.OccurredOn,
            actionType,
            description,
            cancellationToken
            );
    }
}
