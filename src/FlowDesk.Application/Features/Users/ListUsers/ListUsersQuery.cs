using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.Users.ListUsers;

[RequirePermission(Permissions.CanManageWorkspace)]
public record ListUsersQuery(
    Guid WorkspaceId
)  : IRequest<IEnumerable<AppUserDto>>,IWorkspaceRequest;
