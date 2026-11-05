using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Interfaces.Notifications;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Tickets.Events;
using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.EventHandlers.Notifications;

public sealed class TicketStatusChangedNotificationHandler(
    IApplicationDbContext context,
    INotificationService notificationService
) : INotificationHandler<TicketStatusChangedDomainEvent>
{
    public async Task Handle(TicketStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {

        var staffIds = await context.WorkspaceMembers
            .Where(x => x.WorkspaceId == notification.WorkspaceId &&
                (x.Role == WorkspaceRole.Admin || x.Role == WorkspaceRole.Agent))
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        var recipients = staffIds
            .Append(notification.TicketCreatedBy)
            .Where(id => id != Guid.Empty)
            .Distinct();

        await notificationService.CreateAsync(
            notification.WorkspaceId,
            recipients,
            "Status Changed",
            $"Ticket - '{notification.TicketNumber}' status changed from {notification.OldStatus} to {notification.NewStatus}",
            NotificationEventType.TicketStatusChanged,
            notification.TicketId,
            cancellationToken);
    }
}
