using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.ResolveTicket;

[RequirePermission(Permissions.CanChangeTicketStatus)]
public record ResolveTicketCommand(
    Guid TicketId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;