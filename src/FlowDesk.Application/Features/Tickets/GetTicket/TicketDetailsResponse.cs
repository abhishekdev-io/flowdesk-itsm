using FlowDesk.Application.Common.Dto;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.GetTicket;

public record TicketDetailsResponse(
    Guid Id,
    string Title,
    string TicketNumber,
    string Description,
    string? AssetName,
    TicketStatus Status,
    TicketPriority Priority,
    SlaRiskLevel RiskLevel,
    string TimeRemaining,
    UserSummaryDto CreatedByUser,
    UserSummaryDto? AssignedToUser,
    DateTime CreatedAt,
    DateTime? ArchivedAt,
    DateTime? UpdatedAt,
    DateTime? SlaDeadlineAt
);
   