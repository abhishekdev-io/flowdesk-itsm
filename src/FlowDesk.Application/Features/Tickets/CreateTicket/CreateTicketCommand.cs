using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.CreateTicket;

[RequirePermission(Permissions.CanCreateTickets)]
public record CreateTicketCommand(
    Guid WorkspaceId,
    string Title,
    string Description,
    Guid? AssetId,
    TicketType Type,
    TicketPriority Priority
) : IRequest<Guid>,IWorkspaceRequest;
