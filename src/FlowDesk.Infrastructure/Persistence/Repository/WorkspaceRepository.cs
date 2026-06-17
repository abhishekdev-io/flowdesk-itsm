using FlowDesk.Domain.Workspaces;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Infrastructure.Persistence.Repository;

public class WorkspaceRepository(ApplicationDbContext context) : IWorkspaceRepository
{
    private readonly ApplicationDbContext _context = context;
    public Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Workspaces
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public Task<Workspace?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Workspaces
                .Include(w => w.WorkspaceMembers)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public void RemoveAsync(Workspace workspace)
    {
        _context.Workspaces.Remove(workspace);
    }

    public async Task AddAsync(Workspace workspace, CancellationToken cancellationToken)
    {
        await _context.Workspaces.AddAsync(workspace, cancellationToken);
    }

    public async Task<bool> ExistByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Workspaces
        .AsNoTracking()
        .AnyAsync(w => w.Name.Value == name, cancellationToken);
    }

}
