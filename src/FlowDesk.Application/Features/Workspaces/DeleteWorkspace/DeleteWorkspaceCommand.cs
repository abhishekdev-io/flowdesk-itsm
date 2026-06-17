using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.DeleteWorkspace;

[RequirePermission(Permissions.CanManageWorkspace)]
public record DeleteWorkspaceCommand(
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;
