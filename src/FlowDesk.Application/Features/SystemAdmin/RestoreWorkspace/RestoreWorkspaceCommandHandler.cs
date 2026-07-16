using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Workspaces;
using MediatR;

namespace FlowDesk.Application.Features.SystemAdmin.RestoreWorkspace;

public class RestoreWorkspaceCommandHandler(
    IApplicationDbContext context,
    IWorkspaceRepository workspaceRepository
) : IRequestHandler<RestoreWorkspaceCommand,Unit>
{
    public async Task<Unit> Handle(RestoreWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId,cancellationToken)
            ?? throw new NotFoundException("workspace",request.WorkspaceId);
        workspace.Restore();
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;   

    }

} 
