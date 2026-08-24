using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.ListTickets;

public record ListTicketsQuery(
    Guid WorkspaceId,
    TicketFilterParams? Filters = null
) : IRequest<PagedResult<TicketSummaryResponse>>, IWorkspaceRequest;

public record TicketFilterParams : QueryParameters
{   public TicketPriority? Priority { get; init; }
    public TicketStatus? Status { get; init; }
    public TicketType? Type { get; init; }
    public string? Search { get; init; }
    public Guid? AssignedToUserId { get; set; }
    public bool? IsArchived { get; init; } = false;
    public bool? IsUnassigned { get; init; }
};
