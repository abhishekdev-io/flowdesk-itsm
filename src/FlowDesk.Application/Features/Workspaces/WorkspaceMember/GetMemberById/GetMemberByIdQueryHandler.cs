using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.GetMemberById;

public class GetMemberByIdQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetMemberByIdQuery, WorkspaceMemberDto>
{
    public async Task<WorkspaceMemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.WorkspaceUsersView
            .Where(wv => wv.UserId == request.UserId && 
                wv.WorkspaceId == request.WorkspaceId)
            .Select(u => new WorkspaceMemberDto(
                u.UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,
                u.Role
            )).FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("WorkspaceMember",request.UserId);    
    }
}

