using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.TicketComments.RestoreComment;

public class RestoreCommentCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context
) : IRequestHandler<RestoreCommentCommand, Unit>
{
    public async Task<Unit> Handle(RestoreCommentCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdWithCommentsAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        ticket.RestoreComment(request.CommentId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
