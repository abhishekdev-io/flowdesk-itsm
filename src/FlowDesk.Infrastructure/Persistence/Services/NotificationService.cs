using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Notifications;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Services;

public sealed class NotificationService(
    ApplicationDbContext context)
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
        var notifications = recipientIds
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
        var notification = UserNotification.Create(
            workspaceId,
            recipientId?? Guid.Empty,
            title,
            body,
            eventType,
            relatedTicketId);

        context.UserNotifications.Add(notification);
        await context.SaveChangesAsync(cancellationToken);
    }
}
