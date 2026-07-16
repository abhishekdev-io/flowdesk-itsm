using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Workspaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.RemoveMember;

public class RemoveMemberCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IApplicationDbContext context
) : IRequestHandler<RemoveMemberCommand,Unit>
{
    public async Task<Unit> Handle(RemoveMemberCommand request,CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdWithMembersAsync(request.WorkspaceId,cancellationToken)
            ?? throw new NotFoundException("workspace",request.WorkspaceId);

        workspace.RemoveMember(request.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;    
    }

}
 