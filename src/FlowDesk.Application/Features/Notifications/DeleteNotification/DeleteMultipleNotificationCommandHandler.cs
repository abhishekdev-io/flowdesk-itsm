using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Notifications.DeleteNotification;

public class DeleteMultipleNotificationsCommandHandler(
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<DeleteMultipleNotificationsCommand>
{
    public async Task Handle(DeleteMultipleNotificationsCommand request, CancellationToken cancellationToken)
    {
        await context.UserNotifications
            .Where(n => n.WorkspaceId == request.WorkspaceId
                     && n.RecipientUserId == user.UserId
                     && request.NotificationIds.Contains(n.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
