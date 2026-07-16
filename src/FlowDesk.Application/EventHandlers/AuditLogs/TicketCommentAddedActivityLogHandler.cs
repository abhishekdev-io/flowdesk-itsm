using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;

namespace FlowDesk.Application.EventHandlers.AuditLogs;

public sealed class TicketCommentAddedActivityLogHandler(
    ITicketActivityService ticketActivity) 
    : INotificationHandler<TicketCommentAddedDomainEvent>
{
    public async Task Handle(TicketCommentAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        await ticketActivity.LogActivityAsync(
            notification.TicketId,
            notification.CreatedBy,
            notification.OccurredOn,
            TicketActivityType.CommentAdded,
            "A new comment was added to the ticket.",
            cancellationToken
        );
    }
}