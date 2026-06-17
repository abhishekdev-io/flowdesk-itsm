using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Interface;

public interface ITicketActivityService
{
    Task LogActivityAsync(
        Guid TicketId,
        Guid PerformedBy,
        DateTime OccuredAt,
        TicketActivityType Action,
        string Description,
        CancellationToken cancellationToken);
}
