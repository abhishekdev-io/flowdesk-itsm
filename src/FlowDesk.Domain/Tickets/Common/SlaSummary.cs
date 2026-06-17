namespace FlowDesk.Domain.Tickets.Common;

public record SlaSummary
(
    int Total,
    int Breached,
    int Critical,
    int AtRisk,
    int OnTrack,
    int NoSla,
    double CompliancePercent
);
