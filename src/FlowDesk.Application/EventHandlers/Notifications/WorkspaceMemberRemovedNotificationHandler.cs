using FlowDesk.Application.Interface;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Workspaces.Events;

namespace FlowDesk.Application.EventHandlers.Notifications;

public sealed class WorkspaceMemberRemovedNotificationHandler(
    INotificationService notificationService
) : INotificationHandler<WorkspaceMemberRemovedDomainEvent>
{
    public async Task Handle(WorkspaceMemberRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.CreateAsync(
            notification.WorkspaceId,
            notification.RemovedUserId,
            "Removed From Workspace",
            $"You have been removed from workspace - {notification.WorkspaceName.Value}",
            NotificationEventType.RemovedFromWorkspace,
            null,
            cancellationToken
        );
    }
}
