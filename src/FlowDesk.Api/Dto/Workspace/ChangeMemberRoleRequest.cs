using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Api.Dto.Workspace;

public sealed record ChangeMemberRoleRequest(
    WorkspaceRole Role
);
