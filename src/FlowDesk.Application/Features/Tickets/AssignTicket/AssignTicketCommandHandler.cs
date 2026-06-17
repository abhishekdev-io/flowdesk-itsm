using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.AssignTicket;

public class AssignTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
    ) : IRequestHandler<AssignTicketCommand, Unit>
{
    public async Task<Unit> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        ticket.Assign(request.AssigneeId,user.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

