using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Services;
using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.Features.Tickets.GetTicket;

public class GetTicketByIdQueryHandler(
    IApplicationDbContext context,
    ICurrentUserContext currentUserContext,
    ICurrentWorkspaceContext currentWorkspaceContext,
    SlaService slaService
    ) : IRequestHandler<GetTicketByIdQuery, TicketDetailsResponse>
{
    public async Task<TicketDetailsResponse> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var targetTicketData = await context.Tickets
        .AsNoTracking()
        .Where(t => t.Id == request.TicketId && t.WorkspaceId == request.WorkspaceId)
        .Select(t => new
        {
            Ticket = t,
            AssetName = t.Asset != null ? t.Asset.Name.Value : null,
            
            CreatedByUser = context.WorkspaceUsersView
                .Where(wv => wv.UserId == t.CreatedByUserId && wv.WorkspaceId == t.WorkspaceId)
                .Select(u => new UserSummaryDto(u.UserId, u.UserName, u.Email, u.Role, u.AvatarUrl))
                .FirstOrDefault(),

            AssignedToUser = t.AssignedToUserId == null ? null 
                : context.WorkspaceUsersView
                    .Where(wv => wv.UserId == t.AssignedToUserId && wv.WorkspaceId == t.WorkspaceId)
                    .Select(u => new UserSummaryDto(u.UserId, u.UserName, u.Email, u.Role, u.AvatarUrl))
                    .FirstOrDefault()
        })
        .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Ticket not found.", request.TicketId);
        

        var currentUserId = currentUserContext.UserId;
        var currentRole = currentWorkspaceContext.Role;

        var canView = currentRole switch
        {
            WorkspaceRole.Admin => true,
            WorkspaceRole.Agent => true,
            WorkspaceRole.Member =>  targetTicketData.Ticket.CreatedByUserId == currentUserId,
            _ => false
        };

        if (!canView)
        {
            throw new ForbiddenException("You do not have permission to view this ticket.");
        }
        
        var ticket = targetTicketData.Ticket;
        var slaRiskLevel = slaService.GetRiskLevel(ticket);
        string timeRemaining = slaService.GetRemainingTimeDisplay(ticket);

        return new TicketDetailsResponse(
            ticket.Id,
            ticket.Title.Value,
            ticket.TicketNumber,
            ticket.Description.Value,
            ticket.Asset?.Name.Value,
            ticket.Status,
            ticket.Priority,
            slaRiskLevel,
            timeRemaining,
            targetTicketData.CreatedByUser!,
            targetTicketData.AssignedToUser,
            ticket.CreatedAt,
            ticket.ArchivedAt,
            ticket.UpdatedAt,
            ticket.SlaDeadlineAt
        );
    }
}

