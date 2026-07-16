using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.UnAssignTicket;

[RequirePermission(Permissions.CanAssignAndUnAssignTickets)]
public record UnAssignTicketCommand(
    Guid TicketId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;
