using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.UnArchiveTicket;

public class UnArchiveTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
    ) : IRequestHandler<UnArchiveTicketCommand, Unit>
{
    public async Task<Unit> Handle(UnArchiveTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        ticket.UnArchive(user.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

