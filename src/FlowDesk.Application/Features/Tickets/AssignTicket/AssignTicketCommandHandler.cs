using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.AssignTicket;

public class AssignTicketCommandHandler(
    ITicketRepository ticketRepository,
    IApplicationDbContext context,
    ICurrentUserContext user
    ) : IRequestHandler<AssignTicketCommand, Unit>
{
    private const int MaxActiveTicketsPerAssignee = 10;

    public async Task<Unit> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(
            request.TicketId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException($"Ticket",request.TicketId);

        var isWorkspaceMember = await context.WorkspaceMembers.AnyAsync(
            m => m.WorkspaceId == request.WorkspaceId && m.UserId == request.AssigneeId,
            cancellationToken);

        if (!isWorkspaceMember)
            throw new NotFoundException("Workspace member", request.AssigneeId);

        var activeTickets = await context.Tickets.CountAsync(
            t => t.WorkspaceId == request.WorkspaceId
                && t.AssignedToUserId == request.AssigneeId
                && t.Status == TicketStatus.InProgress,
            cancellationToken);

        if (activeTickets >= MaxActiveTicketsPerAssignee)
            throw new ConflictException(
                $"Assignee already has {activeTickets} tickets in progress.");

        ticket.Assign(request.AssigneeId,user.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

