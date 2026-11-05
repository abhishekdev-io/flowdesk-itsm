using FlowDesk.Application.Features.Notifications.GetUserNotifications;

namespace FlowDesk.Application.Interfaces.Notifications;

public interface IRealtimeNotificationPublisher
{
    Task SendToUserAsync(
        Guid workspaceId,
        Guid recipientId,
        NotificationDto notification,
        CancellationToken cancellationToken);
}
  