using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.ReopenTicket;

[RequirePermission(Permissions.CanChangeTicketStatus)]
public record ReopenTicketCommand(
    Guid TicketId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;