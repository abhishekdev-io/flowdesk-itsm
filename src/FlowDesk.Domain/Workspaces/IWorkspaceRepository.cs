using FlowDesk.Domain.Workspaces.Aggregates;

namespace FlowDesk.Domain.Workspaces;
public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Workspace?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistByNameAsync(string name, CancellationToken cancellationToken);
    Task  AddAsync(Workspace workspace, CancellationToken cancellationToken);
    void RemoveAsync(Workspace workspace);
}