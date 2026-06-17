namespace FlowDesk.Api.Dto.Notification;

public record MarkAsReadRequest(List<Guid> NotificationIds);
