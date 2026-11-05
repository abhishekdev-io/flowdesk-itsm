using FlowDesk.Domain.Notifications;

namespace FlowDesk.Application.Interfaces.Notifications;

public interface INotificationService
{
    Task CreateAsync(
        Guid workspaceId,
        IEnumerable<Guid> recipientIds,
        string title,
        string body,
        NotificationEventType eventType,
        Guid? relatedTicketId,
        CancellationToken cancellationToken);

    Task CreateAsync(
        Guid workspaceId,
        Guid? recipientId,
        string title,
        string body,
        NotificationEventType eventType,
        Guid? relatedTicketId,
        CancellationToken cancellationToken);
   
}
 