using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.CloseTicket;

public class CloseTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user,
    ICurrentWorkspaceContext workspaceContext
    ) : IRequestHandler<CloseTicketCommand, Unit>
{
    public async Task<Unit> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        var canClose =
            workspaceContext.IsAdmin ||
            ticket.CreatedByUserId == user.UserId;

        if (!canClose)
            throw new ForbiddenException("Only the ticket creator or an admin can close this ticket.");

        ticket.Close(user.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

 