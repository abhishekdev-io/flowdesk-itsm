using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.ChangePriority;

[RequirePermission(Permissions.CanChangeTicketPriority)]
public record ChangePriorityCommand(
    Guid TicketId,
    TicketPriority NewPriority,
    Guid WorkspaceId
) : IRequest<Unit>, IWorkspaceRequest;
