using FlowDesk.Application.Interfaces.Notifications;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.Notifications;

public sealed class TicketUnAssignNotificationHandler(
    INotificationService notificationService
) : INotificationHandler<TicketUnAssignDomainEvent>
{
    public async Task Handle(TicketUnAssignDomainEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.CreateAsync(
            notification.WorkspaceId,
            notification.WasAssignedTo,
            "Ticket Unassigned",
            $"You have been unassigned Ticket '{notification.Title.Value}'",
            NotificationEventType.TicketUnassigned,
            notification.TicketId,
            cancellationToken);        
    }
}