using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Tickets.TicketComments.EditComment;

public record EditCommentCommand(
    Guid TicketId,
    Guid CommentId,
    string Description,
    Guid WorkspaceId
    ) : IRequest<Unit>, IWorkspaceRequest;