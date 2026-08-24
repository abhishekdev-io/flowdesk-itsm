using FlowDesk.Domain.Tickets;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Repository;

public class TicketRepository(ApplicationDbContext context) : ITicketRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<Ticket?> GetByIdAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken)
    {
        return _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == Id &&
                    t.WorkspaceId == WorkspaceId,
                    cancellationToken
                );
    }

    public Task<Ticket?> GetByIdWithActivityAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken)
    {
        return _context.Tickets
                .Include(t => t.Activities)
                .FirstOrDefaultAsync(t => t.Id == Id &&
                    t.WorkspaceId == WorkspaceId,
                    cancellationToken
                );
    }

    public Task<Ticket?> GetByIdWithCommentsAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken)
    {
        return _context.Tickets
                .Include(t => t.Comments)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == Id &&
                    t.WorkspaceId == WorkspaceId,
                    cancellationToken
                );
    }

    public Task<Ticket?> GetByIdWithAllAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken)
    {
        return _context.Tickets
                .Include(t => t.Activities)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == Id &&
                    t.WorkspaceId == WorkspaceId,
                    cancellationToken
                );
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }

    public void RemoveAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
    }

    public async Task<bool> ExistAsync(Guid ticketId, Guid workspaceId, CancellationToken cancellationToken)
    {
        return await _context.Tickets
                .AsNoTracking()
                .AnyAsync(t => t.Id == ticketId &&
                    t.WorkspaceId == workspaceId,
                    cancellationToken);
    }
}
