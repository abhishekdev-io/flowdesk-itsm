namespace FlowDesk.Domain.Notifications;

public sealed class UserNotification : Entity<Guid>
{
    public Guid WorkspaceId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationEventType EventType { get; private set; }
    public Guid? RelatedTicketId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private UserNotification() : base(Guid.Empty) { }

    public static UserNotification Create(
        Guid workspaceId,
        Guid recipientUserId,
        string title,
        string body,
        NotificationEventType eventType,
        Guid? relatedTicketId = null)
    {
        if (workspaceId == Guid.Empty)
            throw new DomainException("Invalid workspace.");

        if (recipientUserId == Guid.Empty)
            throw new DomainException("Invalid recipient.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Notification title is required.");

        if (string.IsNullOrWhiteSpace(body))
            throw new DomainException("Notification body is required.");

        return new UserNotification
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            RecipientUserId = recipientUserId,
            Title = title,
            Body = body,
            EventType = eventType,
            RelatedTicketId = relatedTicketId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (IsRead) return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}
