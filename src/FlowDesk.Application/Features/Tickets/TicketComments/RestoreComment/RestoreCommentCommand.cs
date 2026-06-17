using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.TicketComments.RestoreComment;

[RequirePermission(Permissions.CanRestoreComments)]
public record RestoreCommentCommand(
    Guid TicketId,
    Guid CommentId,
    Guid WorkspaceId
    ) : IRequest<Unit>, IWorkspaceRequest;

