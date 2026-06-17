using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Domain.Tickets.Services;

namespace FlowDesk.Application.Features.Tickets.ListTickets;

public sealed class ListTicketsQueryHandler(
    IApplicationDbContext context, 
    ICurrentWorkspaceContext currentWorkspaceContext,
    ICurrentUserContext currentUserContext,
    SlaService slaService
    ) : IRequestHandler<ListTicketsQuery, PagedResult<TicketSummaryResponse>>
{
    public async Task<PagedResult<TicketSummaryResponse>> Handle(ListTicketsQuery request, CancellationToken cancellationToken)
    {
        var workspaceId = request.WorkspaceId;
        var currentUserId = currentUserContext.UserId;
        var currentRole = currentWorkspaceContext.Role;

        var query = context.Tickets
            .AsNoTracking()
            .Where(t => t.WorkspaceId == workspaceId &&
                !t.IsArchived);

        query = currentRole switch
        {
            WorkspaceRole.Admin => query,
            WorkspaceRole.Agent => query,
            WorkspaceRole.Member => query.Where(t => t.CreatedByUserId == currentUserId),
            _ => throw new ForbiddenException("You do not have permission to view tickets in this workspace.")
        };    

        var f = request.Filters ?? new TicketFilterParams();

        if (f.AssignedToUserId.HasValue)
        {
            query = query.Where(t => t.AssignedToUserId == f.AssignedToUserId.Value);
        }
            if (f.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == f.Priority.Value);
        }
        if (f.Status.HasValue)
        {
            query = query.Where(t => t.Status == f.Status.Value);
        }
        if (f.Type.HasValue)
        {
            query = query.Where(t => t.Type == f.Type.Value);
        }
        if (!string.IsNullOrWhiteSpace(f.Search))
        {
            var search = f.Search.Trim();

            query = query.Where(t =>
                t.TicketNumber.Contains(search) ||
                t.Title.Value.Contains(search) ||
                (t.Description != null && t.Description.Value.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = f.SortBy?.ToLower() switch
        {
            "createdat" => f.Descending ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),

            "priority"  => f.Descending ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),

            "status"    => f.Descending ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),

            "type"      => f.Descending ? query.OrderByDescending(t => t.Type)
                : query.OrderBy(t => t.Type),

            _           => query.OrderByDescending(t => t.CreatedAt)
        };

        var rawTickets = await query
            .Skip((f.Page - 1) * f.PageSize)
            .Take(f.PageSize)
            .Select(t => new
            {
                Ticket = t,

                AssetName = t.Asset != null ? t.Asset.Name.Value : null,

                CreatedByUserName = context.WorkspaceUsersView
                    .Where(u => u.UserId == t.CreatedByUserId &&
                        u.WorkspaceId == t.WorkspaceId)
                    .Select(u => u.UserName)
                    .FirstOrDefault()!,
                
                AssignedToUserName = t.AssignedToUserId == null ? null
                    : context.WorkspaceUsersView
                        .Where(u => u.UserId == t.AssignedToUserId &&
                           u.WorkspaceId == t.WorkspaceId)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
        
        var tickets = rawTickets.Select(x => new TicketSummaryResponse(
            x.Ticket.Id,
            x.Ticket.Title.Value,
            x.Ticket.TicketNumber,
            x.Ticket.Asset?.Name.Value,
            x.CreatedByUserName,
            x.AssignedToUserName,
            x.Ticket.Type,
            x.Ticket.Status,
            x.Ticket.Priority,
            slaService.GetRiskLevel(x.Ticket),
            x.Ticket.ArchivedAt,
            x.Ticket.CreatedAt
        )).ToList();

        return new PagedResult<TicketSummaryResponse>
        {
            Items = tickets,
            Page = f.Page,
            PageSize = f.PageSize,
            TotalCount = totalCount
        };
    }
}

