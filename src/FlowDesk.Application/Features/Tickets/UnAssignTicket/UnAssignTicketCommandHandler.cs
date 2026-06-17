using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.UnAssignTicket;

public class UnAssignTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
    ) : IRequestHandler<UnAssignTicketCommand, Unit>
{
    public async Task<Unit> Handle(UnAssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        ticket.UnAssign(user.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

