using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.UnArchiveTicket;

[RequirePermission(Permissions.CanArchiveAndUnArchiveTickets)]
public record UnArchiveTicketCommand(
    Guid TicketId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;