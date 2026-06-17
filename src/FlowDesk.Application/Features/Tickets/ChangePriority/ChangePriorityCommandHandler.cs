using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.ChangePriority;

public class ChangePriorityCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<ChangePriorityCommand, Unit>
{
    public async Task<Unit> Handle(ChangePriorityCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, request.WorkspaceId, cancellationToken)
            ??  throw new NotFoundException("Ticket", request.TicketId);
        
        ticket.ChangePriority(request.NewPriority, user.UserId);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
