using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Workspaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.DeleteWorkspace;

public class DeleteWorkspaceCommandHandler(
    IApplicationDbContext context,
    IWorkspaceRepository workspaceRepository
) : IRequestHandler<DeleteWorkspaceCommand,Unit>
{
    public async Task<Unit> Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId,cancellationToken)
            ?? throw new NotFoundException("workspace",request.WorkspaceId);

        workspace.Delete();
        await context.SaveChangesAsync(cancellationToken); 
        return Unit.Value;   
    }
}
