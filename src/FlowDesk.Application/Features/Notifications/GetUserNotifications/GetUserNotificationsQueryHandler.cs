using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Notifications.GetUserNotifications;

public class GetUserNotificationsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<GetUserNotificationsQuery, PagedResult<NotificationDto>>
{
    public async Task<PagedResult<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = context.UserNotifications
            .Where(n => n.WorkspaceId == request.WorkspaceId &&
                n.RecipientUserId == user.UserId)
            .OrderByDescending(n => n.CreatedAt);
        
        var totalCount = await query.CountAsync(cancellationToken);

        var notifications = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Body,
                n.EventType.ToString(),
                n.RelatedTicketId,
                n.IsRead,
                n.CreatedAt
            ))
            .ToListAsync(cancellationToken);
        
        return new PagedResult<NotificationDto>
        {
            Items = notifications,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        }; 
    }
}
