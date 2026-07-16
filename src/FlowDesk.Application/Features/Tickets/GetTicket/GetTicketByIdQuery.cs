using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Tickets.GetTicket;

public record GetTicketByIdQuery(
    Guid TicketId,
    Guid WorkspaceId
) : IRequest<TicketDetailsResponse>, IWorkspaceRequest;
