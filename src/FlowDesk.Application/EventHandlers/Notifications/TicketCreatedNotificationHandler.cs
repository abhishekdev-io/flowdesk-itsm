using FlowDesk.Application.Interface;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Tickets.Events;
using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.EventHandlers.Notifications;

public sealed class TicketCreatedNotificationHandler(
    INotificationService notificationService,
    IApplicationDbContext context
) : INotificationHandler<TicketCreatedDomainEvent>
{
    public async Task Handle(TicketCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var recipients = await context.WorkspaceMembers
            .Where(x => x.WorkspaceId == notification.WorkspaceId &&
                    (x.Role == WorkspaceRole.Admin ||
                        x.Role == WorkspaceRole.Agent))
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        await notificationService.CreateAsync(
            notification.WorkspaceId,
            recipients,
            "Ticket Created",
            $"Ticket '{notification.Title.Value}' has been created.",
            NotificationEventType.TicketCreated,
            notification.TicketId,
            cancellationToken);
    }
}
