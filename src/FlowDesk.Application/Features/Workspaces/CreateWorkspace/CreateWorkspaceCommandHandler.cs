using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Workspaces;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.CreateWorkspace;

public class CreateWorkspaceCommandHandler(
    IApplicationDbContext context,
    ICurrentUserContext user,
    IWorkspaceRepository workspaceRepository
) : IRequestHandler<CreateWorkspaceCommand,Guid>
{
    public async Task<Guid> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var exist = await workspaceRepository.ExistByNameAsync(request.Name,cancellationToken);

        if(exist)
         throw new ConflictException($"Workspace with name {request.Name} already exists");

        var workspace = Workspace.Create(
            request.Name,
            request.Description
        ); 
        workspace.AddMember(user.UserId,WorkspaceRole.Admin);

        await workspaceRepository.AddAsync(workspace,cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return workspace.Id;
    }
}
