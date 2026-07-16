using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.CloseTicket;

public class CloseTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
    ) : IRequestHandler<CloseTicketCommand, Unit>
{
    public async Task<Unit> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        ticket.Close(user.UserId,request.Reason);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

 