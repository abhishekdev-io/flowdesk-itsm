using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.ChangeType;

[RequirePermission(Permissions.CanChangeTicketType)]
public record ChangeTypeCommand(
    Guid TicketId,
    TicketType NewType,
    Guid WorkspaceId
) : IRequest<Unit>, IWorkspaceRequest;
