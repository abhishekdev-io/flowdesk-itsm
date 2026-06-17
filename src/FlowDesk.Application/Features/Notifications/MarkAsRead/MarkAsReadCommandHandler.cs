using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Notifications.MarkAsRead;

public class MarkAsReadCommandHandler(
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<MarkAsReadCommand>
{
    public async Task Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        await context.UserNotifications
            .Where(n => n.WorkspaceId == request.WorkspaceId
                     && n.RecipientUserId == user.UserId
                     && request.NotificationIds.Contains(n.Id)
                     && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                cancellationToken);
    }
}
