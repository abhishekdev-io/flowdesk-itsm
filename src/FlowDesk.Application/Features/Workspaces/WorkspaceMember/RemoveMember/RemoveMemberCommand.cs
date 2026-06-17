using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.RemoveMember;

[RequirePermission(Permissions.CanManageWorkspace)]
public record RemoveMemberCommand(
    Guid UserId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;
