using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketCreatedActivityLogHandler(ITicketActivityService ticketActivity)
    : INotificationHandler<TicketCreatedDomainEvent>
{
    public async Task Handle(TicketCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        string titleText = notification.Title.Value;
        var description = $"Ticket '{titleText}' was opened and set to {notification.Priority} priority.";

        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.CreatedBy,
            notification.OccurredOn,
            TicketActivityType.Created,
            description,
            cancellationToken
        );
    }
}
