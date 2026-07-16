using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Notifications.MarkAsRead;

public record MarkAsReadCommand(
    Guid WorkspaceId,
    List<Guid> NotificationIds
) : IRequest, IWorkspaceRequest;
