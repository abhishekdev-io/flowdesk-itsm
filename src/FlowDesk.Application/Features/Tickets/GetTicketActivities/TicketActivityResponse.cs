using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Tickets.GetTicketActivities;

public sealed record TicketActivityResponse(
    Guid Id,
    PerformedbyDto PerformedBy,
    DateTime OccurredAt,
    string Action,
    string Description
);