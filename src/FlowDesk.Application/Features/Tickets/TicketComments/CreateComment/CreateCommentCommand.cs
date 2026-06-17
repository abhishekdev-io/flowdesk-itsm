using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Tickets.TicketComments.CreateComment;

public record CreateCommentCommand(
    Guid TicketId,
    Guid WorkspaceId,
    string Description
) : IRequest<Unit>,IWorkspaceRequest;

