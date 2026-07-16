using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.TicketComments.GetDeletedCommentsByTicketId;

[RequirePermission(Permissions.CanViewDeletedComments)]
public record GetDeletedCommentsByTicketIdQuery(
    Guid TicketId,
    Guid WorkspaceId,
    int Page = 1,
    int PageSize = 20,
    bool Descending = true
) : IRequest<PagedResult<DeletedCommentResponse>>,IWorkspaceRequest;
