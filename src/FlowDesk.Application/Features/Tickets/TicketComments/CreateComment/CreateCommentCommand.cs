using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Tickets.TicketComments.CreateComment;

public record CreateCommentCommand(
    Guid TicketId,
    Guid WorkspaceId,
    string Description
) : IRequest<Unit>,IWorkspaceRequest;

