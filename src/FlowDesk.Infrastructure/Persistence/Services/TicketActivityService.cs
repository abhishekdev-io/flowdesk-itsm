using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Services;

public sealed class TicketActivityService(ApplicationDbContext context) : ITicketActivityService
{
    public async Task LogActivityAsync(
        Guid TicketId,
        Guid PerformedBy,
        DateTime OccuredAt,
        TicketActivityType Action,
        string Description,
        CancellationToken cancellationToken)
    {
        var log = TicketActivity.Create(
            TicketId,
            PerformedBy,
            OccuredAt,
            Action,
            Description);

        context.TicketActivities.Add(log);

        await context.SaveChangesAsync(cancellationToken);
    }
}
