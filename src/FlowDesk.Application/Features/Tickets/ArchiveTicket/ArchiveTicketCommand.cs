using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.ArchiveTicket;

[RequirePermission(Permissions.CanArchiveAndUnArchiveTickets)]
public record ArchiveTicketCommand(
    Guid TicketId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;