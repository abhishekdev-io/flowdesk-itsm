using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.TicketComments.CreateComment;

public class CreateCommentCommandHandler(
    ITicketRepository ticketRepository,
    ICurrentUserContext user,
    ICurrentWorkspaceContext workspaceContext,
    IApplicationDbContext context
) : IRequestHandler<CreateCommentCommand,Unit>
{
    private const int MaxCommentsPerTicket = 200;

    public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {

        var ticket = await ticketRepository.GetByIdWithCommentsAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        var isAllowed =
            workspaceContext.IsAdmin || 
            workspaceContext.IsAgent || 
            ticket.CreatedByUserId == user.UserId;   

        if (!isAllowed)
        throw new ForbiddenException("Cannot Create Comment for this ticket"); 

        if (ticket.Comments.Count >= MaxCommentsPerTicket)
            throw new ConflictException("This ticket has reached the maximum number of comments.");

        ticket.AddComment(request.Description, user.UserId);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;      
    }
}
