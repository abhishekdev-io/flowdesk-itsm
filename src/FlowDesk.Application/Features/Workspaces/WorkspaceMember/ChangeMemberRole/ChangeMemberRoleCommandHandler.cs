using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Workspaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.ChangeMemberRole;

public class AddWorkspaceMemberCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IApplicationDbContext context
) : IRequestHandler<ChangeMemberRoleCommand,Unit>
{
    public async Task<Unit> Handle(ChangeMemberRoleCommand request,CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdWithMembersAsync(request.WorkspaceId,cancellationToken)
            ?? throw new NotFoundException("workspace",request.WorkspaceId);

        workspace.ChangeMemberRole(request.UserId,request.Role);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;    
    }

}
