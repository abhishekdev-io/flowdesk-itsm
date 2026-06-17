using FlowDesk.Domain.Workspaces.ValueObjects;

namespace FlowDesk.Domain.Workspaces.Events;

public sealed record WorkspaceMemberRemovedDomainEvent(
    Guid WorkspaceId,
    Guid RemovedUserId,
    WorkspaceName WorkspaceName
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
