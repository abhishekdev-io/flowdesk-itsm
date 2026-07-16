using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Workspaces.Enums;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.AddMember;

[RequirePermission(Permissions.CanManageWorkspace)]
public record AddMemberCommand(
    Guid UserId,
    Guid WorkspaceId,
    WorkspaceRole Role
) : IRequest<Unit>,IWorkspaceRequest;
 