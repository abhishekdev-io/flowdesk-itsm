using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.TicketComments.DeleteComment;

public class DeleteCommentCommandHandler(
    ITicketRepository ticketRepository,
    ICurrentUserContext user,
    ICurrentWorkspaceContext workspaceContext,
    IApplicationDbContext context
) : IRequestHandler<DeleteCommentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdWithCommentsAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        var userRole = workspaceContext.Role;

        ticket.DeleteComment(request.CommentId, user.UserId, userRole);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
