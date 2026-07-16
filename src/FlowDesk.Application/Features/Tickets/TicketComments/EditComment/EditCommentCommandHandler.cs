using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.TicketComments.EditComment;

public class EditCommentCommandHandler(
    ITicketRepository ticketRepository,
    ICurrentUserContext user,
    IApplicationDbContext context
) : IRequestHandler<EditCommentCommand, Unit>
{
    public async Task<Unit> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdWithCommentsAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        ticket.EditComment(request.CommentId, user.UserId, request.Description);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
