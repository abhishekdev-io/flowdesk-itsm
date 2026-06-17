using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Application.Common.Dto;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.ListMembers;

[RequirePermission(Permissions.CanManageWorkspace)]
public record ListMembersQuery(
    Guid WorkspaceId
) : IRequest<IEnumerable<WorkspaceMemberDto>>,IWorkspaceRequest;