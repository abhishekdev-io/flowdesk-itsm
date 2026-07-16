using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Workspaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.AddMember;

public class AddMemberCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IApplicationDbContext context
) : IRequestHandler<AddMemberCommand,Unit>
{
    public async Task<Unit> Handle(AddMemberCommand request,CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdWithMembersAsync(request.WorkspaceId,cancellationToken)
            ?? throw new NotFoundException("workspace",request.WorkspaceId);

        workspace.AddMember(request.UserId,request.Role);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;    
    }
}

