using FlowDesk.Domain.Tickets.Aggregates;

namespace FlowDesk.Domain.Tickets;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdWithCommentsAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdWithActivityAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdWithAllAsync(Guid Id, Guid WorkspaceId, CancellationToken cancellationToken);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<bool> ExistAsync(Guid ticketId, Guid workspaceId, CancellationToken cancellationToken);
    void RemoveAsync(Ticket ticket);
}

