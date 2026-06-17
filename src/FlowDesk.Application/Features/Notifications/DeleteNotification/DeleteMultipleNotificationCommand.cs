using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Notifications.DeleteNotification;

public sealed record DeleteMultipleNotificationsCommand(
    Guid WorkspaceId,
    List<Guid> NotificationIds
) : IRequest, IWorkspaceRequest;
