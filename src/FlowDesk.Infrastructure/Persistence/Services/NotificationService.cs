using FlowDesk.Application.Features.Notifications.GetUserNotifications;
using FlowDesk.Application.Interfaces.Notifications;
using FlowDesk.Domain.Notifications;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Services;

public sealed class NotificationService(
    ApplicationDbContext context,
    IRealtimeNotificationPublisher realtimePublisher)
    : INotificationService
{
    public async Task CreateAsync(
        Guid workspaceId,
        IEnumerable<Guid> recipientIds,
        string title,
        string body,
        NotificationEventType eventType,
        Guid? relatedTicketId,
        CancellationToken cancellationToken)
    {
        var recipients = recipientIds?
            .Distinct()
            .ToList();

        if (recipients is null || recipients.Count == 0)
        {
            throw new ArgumentException(
                "Recipient IDs cannot be null or empty.",
                nameof(recipientIds));
        }

        var notifications = recipients
            .Select(recipientId =>
                UserNotification.Create(
                    workspaceId,
                    recipientId,
                    title,
                    body,
                    eventType,
                    relatedTicketId))
            .ToList();

        context.UserNotifications.AddRange(notifications);
        await context.SaveChangesAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            var dto = new NotificationDto(
                notification.Id,
                notification.Title,
                notification.Body,
                notification.EventType.ToString(),
                notification.RelatedTicketId,
                notification.IsRead,
                notification.CreatedAt
            );

            await realtimePublisher.SendToUserAsync(
                workspaceId,
                notification.RecipientUserId,
                dto,
                cancellationToken);
        }
    }

    public async Task CreateAsync(
    Guid workspaceId,
    Guid? recipientId,
    string title,
    string body,
    NotificationEventType eventType,
    Guid? relatedTicketId,
    CancellationToken cancellationToken)
    {
        if(recipientId == null)
        {
            throw new ArgumentException("Recipient ID cannot be null.", nameof(recipientId));
        }

        var notification = UserNotification.Create(
            workspaceId,
            recipientId?? Guid.Empty,
            title,
            body,
            eventType,
            relatedTicketId);

        context.UserNotifications.Add(notification);
        await context.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto(
            notification.Id,
            notification.Title,
            notification.Body,
            notification.EventType.ToString(),
            notification.RelatedTicketId,
            notification.IsRead,
            notification.CreatedAt
        );

        await realtimePublisher.SendToUserAsync(
            workspaceId,
            notification.RecipientUserId,
            dto,
            cancellationToken);
    }
}
  