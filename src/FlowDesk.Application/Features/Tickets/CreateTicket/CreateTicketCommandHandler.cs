using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets.Aggregates;

namespace FlowDesk.Application.Features.Tickets.CreateTicket;

public class CreateTicketCommandHandler(
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<CreateTicketCommand,Guid>
{
    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = Ticket.Create(
            request.Title,
            request.Description,
            request.WorkspaceId,
            request.Type,
            request.Priority,
            user.UserId,            
            request.AssetId            
        );

        await context.Tickets.AddAsync(ticket, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
 