using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Notifications.MarkAsRead;

public record MarkAsReadCommand(
    Guid WorkspaceId,
    List<Guid> NotificationIds
) : IRequest, IWorkspaceRequest;
