using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.ListMembers;

public class ListWorkspaceMemberQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<ListMembersQuery, IEnumerable<WorkspaceMemberDto>>
{
    public async Task<IEnumerable<WorkspaceMemberDto>> Handle(ListMembersQuery request, CancellationToken cancellationToken)
    {
        return await context.WorkspaceUsersView
            .Where(wv => wv.WorkspaceId == request.WorkspaceId)
            .Select(u => new WorkspaceMemberDto(
                u.UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,
                u.Role
            )).ToListAsync(cancellationToken);
    }
}
