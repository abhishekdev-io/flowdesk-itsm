using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Tickets.TicketComments.GetCommentsByTicketId;

public class GetCommentsByTicketIdQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetCommentsByTicketIdQuery,PagedResult<CommentResponse>>
{
    public async Task<PagedResult<CommentResponse>> Handle(GetCommentsByTicketIdQuery request, CancellationToken cancellationToken)
    {
        var query = context.TicketComments
            .Where(tc => tc.TicketId == request.TicketId);

        query = request.Descending ? query.OrderByDescending(tc => tc.CreatedAt)
            : query.OrderBy(tc => tc.CreatedAt);   

        var totalCount = await query.CountAsync(cancellationToken);

        var comments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tc => new CommentResponse(
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
                tc.IsEdited
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<CommentResponse>
        {
            Items = comments,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };    
    }
}