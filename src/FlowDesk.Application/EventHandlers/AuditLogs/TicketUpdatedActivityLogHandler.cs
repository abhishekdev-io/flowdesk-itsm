using System.Text;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketUpdatedActivityLogHandler(
    ITicketActivityService ticketActivity)
    : INotificationHandler<TicketUpdatedDomainEvent>
{
    public async Task Handle(TicketUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var logBuilder = new StringBuilder("Ticket details were updated: ");
        var changes = new List<string>();

        if (notification.OldTitle != notification.NewTitle)
        {
            changes.Add($"Title changed from '{notification.OldTitle.Value}' to '{notification.NewTitle.Value}'");
        }
        if (notification.OldDescription != notification.NewDescription)
        {
            changes.Add("the description was modified");
        }

        logBuilder.Append(string.Join(", ", changes));

        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.UpdatedBy,
            notification.OccurredOn,
            TicketActivityType.Updated,
            logBuilder.ToString(),
            cancellationToken
        );
    }
}
