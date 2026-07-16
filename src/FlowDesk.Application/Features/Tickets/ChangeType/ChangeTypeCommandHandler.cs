using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.ChangeType;

public class ChangeTypeCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<ChangeTypeCommand, Unit>
{
    public async Task<Unit> Handle(ChangeTypeCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, request.WorkspaceId, cancellationToken)
            ??  throw new NotFoundException("Ticket", request.TicketId);
        
        ticket.ChangeType(request.NewType, user.UserId);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
