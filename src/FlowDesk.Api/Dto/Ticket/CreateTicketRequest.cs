using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Api.Dto.Ticket;

public record CreateTicketRequest(
    string Title,
    string Description,
    Guid? AssetId,
    TicketType Type,
    TicketPriority Priority
);
