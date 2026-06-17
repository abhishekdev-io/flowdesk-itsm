using FlowDesk.Application.Interface;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Workspaces.Events;

namespace FlowDesk.Application.EventHandlers.Notifications;

public sealed class WorkspaceRoleChangedNotificationHandler(
    INotificationService notificationService
) : INotificationHandler<WorkspaceRoleChangedDomainEvent>
{
    public async Task Handle(WorkspaceRoleChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.CreateAsync(
            notification.WorkspaceId,
            notification.RoleChangedUserId,
            "Workspace Role Changed",
            $"Your Role changed to {notification.CurrentRole} in workspace - {notification.WorkspaceName.Value}",
            NotificationEventType.WorkspaceRoleChanged,
            null,
            cancellationToken
        );
    }
}
