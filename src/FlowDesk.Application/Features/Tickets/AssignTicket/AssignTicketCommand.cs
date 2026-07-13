using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.AssignTicket;

[RequirePermission(Permissions.CanAssignAndUnAssignTickets)]
public record AssignTicketCommand(
    Guid TicketId,
    Guid AssigneeId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest; 