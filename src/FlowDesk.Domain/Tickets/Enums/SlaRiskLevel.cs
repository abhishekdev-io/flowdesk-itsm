namespace FlowDesk.Domain.Tickets.Enums;

public enum SlaRiskLevel
{
    NoSla = 1,
    OnTrack = 2,    // > 50% time remaining
    AtRisk = 3,     // 20–50% remaining
    Critical = 4,   // < 20% remaining
    Breached = 5
}
