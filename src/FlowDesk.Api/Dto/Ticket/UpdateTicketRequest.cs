namespace FlowDesk.Api.Dto.Ticket;

public record UpdateTicketRequest(
    string Title,
    string Description
);
