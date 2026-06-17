using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketStatusChangedActivityLogHandler(
    ITicketActivityService ticketActivity) 
    : INotificationHandler<TicketStatusChangedDomainEvent>
{
    public async Task Handle(TicketStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        string description = (notification.OldStatus, notification.NewStatus) switch
        {
            (TicketStatus.Resolved, TicketStatus.InProgress) => "Ticket was re-opened and returned to In Progress state.",
            
            (TicketStatus.Resolved, TicketStatus.Open) => "Ticket was re-opened.",

            (_, TicketStatus.InProgress) => "Ticket status updated to In Progress.",
            (_, TicketStatus.Resolved) => "Ticket was marked as Resolved.",
            (_, TicketStatus.Closed) => "Ticket was officially Closed.",
            _ => $"Ticket status changed from {notification.OldStatus} to {notification.NewStatus}."
        };

        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.ChangedBy,
            notification.OccurredOn,
            TicketActivityType.StatusChanged,
            description,
            cancellationToken
        );
    }
}