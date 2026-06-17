using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Tickets.UpdateTicket;

public record UpdateTicketCommand(
    Guid Id,
    Guid WorkspaceId,
    string Title,
    string Description
) : IRequest<Unit>, IWorkspaceRequest;
