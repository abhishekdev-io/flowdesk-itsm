namespace FlowDesk.Application.Interfaces.Realtime;

public interface IMembershipChecker
{
    Task<bool> IsMemberAsync (Guid workspaceId, Guid userId,
    CancellationToken cancellationToken);
}
