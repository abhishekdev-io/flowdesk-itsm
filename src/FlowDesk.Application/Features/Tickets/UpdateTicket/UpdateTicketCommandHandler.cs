using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;
using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.Features.Tickets.UpdateTicket;

public class UpdateTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user,
    ICurrentWorkspaceContext currentWorkspaceContext
) : IRequestHandler<UpdateTicketCommand,Unit>
{
    public async Task<Unit> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
                request.Id,
                request.WorkspaceId,
                cancellationToken)
                    ?? throw new NotFoundException("ticket",request.Id);
        
        bool isCreator = ticket.CreatedByUserId == user.UserId;
        bool isPrivileged = currentWorkspaceContext.Role is WorkspaceRole.Admin or WorkspaceRole.Agent;

        if (!isCreator && !isPrivileged)
            throw new ForbiddenException("You do not have permission to update this ticket.");

        ticket.Update(
            request.Title,
            request.Description,
            user.UserId
        );
        await context.SaveChangesAsync(cancellationToken);  
        return Unit.Value;  
    }
}
