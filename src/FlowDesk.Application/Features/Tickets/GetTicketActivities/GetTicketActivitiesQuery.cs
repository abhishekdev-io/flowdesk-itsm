using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Tickets.GetTicketActivities;

public sealed record GetTicketActivitiesQuery(
    Guid TicketId, 
    Guid WorkspaceId
) : IRequest<IReadOnlyCollection<TicketActivityResponse>>,IWorkspaceRequest;