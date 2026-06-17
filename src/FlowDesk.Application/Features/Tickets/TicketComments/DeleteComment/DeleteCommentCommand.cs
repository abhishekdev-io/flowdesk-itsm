using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Tickets.TicketComments.DeleteComment;

public record DeleteCommentCommand(
    Guid TicketId,
    Guid CommentId,
    Guid WorkspaceId
    ) : IRequest<Unit>, IWorkspaceRequest;
