using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Domain.Workspaces.ValueObjects;

namespace FlowDesk.Domain.Workspaces.Events;

public sealed record WorkspaceRoleChangedDomainEvent(
    Guid WorkspaceId,
    Guid RoleChangedUserId,
    WorkspaceRole CurrentRole,
    WorkspaceName WorkspaceName
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
