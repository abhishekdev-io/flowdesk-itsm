using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
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

        if(await workspaceRepository.ExistByNameAsync(request.Name,cancellationToken))
            throw new ConflictException($"workspace with name {request.Name} already exist"); 

        workspace.Update(
            request.Name,
            request.Description
        );

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;     
    }
}
