using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Tickets.TicketComments.GetDeletedCommentsByTicketId;

public class GetDeletedCommentsByTicketIdQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetDeletedCommentsByTicketIdQuery,PagedResult<DeletedCommentResponse>>
{
    public async Task<PagedResult<DeletedCommentResponse>> Handle(GetDeletedCommentsByTicketIdQuery request, CancellationToken cancellationToken)
    {
        var query = context.TicketComments
            .IgnoreQueryFilters()
            .Where(tc => tc.TicketId == request.TicketId
                && tc.IsDeleted);

        query = request.Descending ? query.OrderByDescending(tc => tc.CreatedAt)
            : query.OrderBy(tc => tc.CreatedAt);   

        var totalCount = await query.CountAsync(cancellationToken);

        var comments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tc => new DeletedCommentResponse(
                tc.Id,
                tc.Description,
                //author details
                context.WorkspaceUsersView
                    .Where(wv => wv.UserId == tc.CreatedBy)
                    .Select(u => new CommentAuthorDto(
                        u.UserName,
                        u.Role,
                        u.AvatarUrl
                    ))
                    .FirstOrDefault()!,

                tc.CreatedAt,
                tc.EditedAt,
                tc.IsEdited,
                tc.DeletedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<DeletedCommentResponse>
        {
            Items = comments,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };    
    }
}
