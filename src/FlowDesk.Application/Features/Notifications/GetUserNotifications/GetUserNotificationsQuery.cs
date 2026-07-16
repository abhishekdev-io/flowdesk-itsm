using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Notifications.GetUserNotifications;

public record GetUserNotificationsQuery(
    Guid WorkspaceId, 
    int Page = 1, 
    int PageSize = 20
) : IRequest<PagedResult<NotificationDto>>,IWorkspaceRequest;

public record NotificationDto(
    Guid Id,
    string Title,
    string Body,
    string EventType,
    Guid? RelatedTicketId,
    bool IsRead,
    DateTime CreatedAt
);