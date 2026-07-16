using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Tickets.CloseTicket;

[RequirePermission(Permissions.CanChangeTicketStatus)]
public record CloseTicketCommand(
    Guid TicketId,
    string? Reason,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest; 