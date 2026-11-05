using FlowDesk.Api.Hubs;
using FlowDesk.Application.Features.Notifications.GetUserNotifications;
using FlowDesk.Application.Interfaces.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace FlowDesk.Api.Realtime;

public sealed class RealtimeNotificationPublisher(
    IHubContext<NotificationHub> hubContext
    ) : IRealtimeNotificationPublisher
{
    public Task SendToUserAsync(
        Guid workspaceId,
        Guid recipientId,
        NotificationDto notification,
        CancellationToken cancellationToken)
    {
        var groupName = $"workspace:{workspaceId}:user:{recipientId}";

        return hubContext.Clients
            .Group(groupName)
            .SendAsync(
                "NotificationReceived",
                notification,
                cancellationToken);
    }
}
 