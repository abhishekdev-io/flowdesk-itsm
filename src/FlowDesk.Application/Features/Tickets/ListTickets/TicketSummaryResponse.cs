using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Application.Features.Tickets.ListTickets;

public record TicketSummaryResponse(
    Guid Id,
    string Title,
    string TicketNumber,
    string? AssetName,          
    string CreatedByUserName,   
    string? AssignedToUserName,
    TicketType Type,
    TicketStatus Status,
    TicketPriority Priority,
    SlaRiskLevel RiskLevel,
    DateTime? ArchivedAt,
    DateTime CreatedAt
);
