namespace FlowDesk.Api.Dto.Notification;

public sealed record DeleteRequest(List<Guid> NotificationIds);
