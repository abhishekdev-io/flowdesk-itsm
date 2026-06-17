using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Workspaces.Enums;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.ChangeMemberRole;

[RequirePermission(Permissions.CanManageWorkspace)]
public record ChangeMemberRoleCommand(
    Guid UserId,
    Guid WorkspaceId,
    WorkspaceRole Role
) : IRequest<Unit>,IWorkspaceRequest;
