using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Workspaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.UpdateWorkspace;

public class UpdateWorkspaceCommandHandler(
    IApplicationDbContext context,
    IWorkspaceRepository workspaceRepository) : IRequestHandler<UpdateWorkspaceCommand,Unit>
{
    public async Task<Unit> Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId,cancellationToken)
            ?? throw new NotFoundException("workspace",request.WorkspaceId);

        if (!string.Equals(workspace.Name.Value, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await workspaceRepository.ExistByNameAsync(request.Name, cancellationToken))
                throw new ConflictException($"Workspace with name '{request.Name}' already exists.");
        } 

        workspace.Update(
            request.Name,
            request.Description
        );

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;     
    }
}
