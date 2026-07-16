using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.UpdateWorkspace;

[RequirePermission(Permissions.CanManageWorkspace)]
public record UpdateWorkspaceCommand(
    Guid WorkspaceId,
    string Name,
    string Description
) : IRequest<Unit>,IWorkspaceRequest;
