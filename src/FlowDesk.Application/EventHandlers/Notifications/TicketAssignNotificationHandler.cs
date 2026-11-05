using FlowDesk.Application.Interfaces.Notifications;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.Notifications;

public sealed class TicketAssignNotificationHandler(
    INotificationService notificationService
) : INotificationHandler<TicketAssignDomainEvent>
{
    public async Task Handle(TicketAssignDomainEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.CreateAsync(
            notification.WorkspaceId,
            notification.AssignedToId,
            "Ticket Assigned",
            $"Ticket '{notification.Title.Value}' has been assigned to you.",
            NotificationEventType.TicketAssigned,
            notification.TicketId,
            cancellationToken);        
    }
}
