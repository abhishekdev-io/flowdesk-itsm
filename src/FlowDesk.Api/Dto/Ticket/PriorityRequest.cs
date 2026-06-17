using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Api.Dto.Ticket;

public record PriorityRequest(
    TicketPriority Priority
);
