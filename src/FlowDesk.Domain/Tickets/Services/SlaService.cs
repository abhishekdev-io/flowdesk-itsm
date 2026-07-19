using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Common;
using FlowDesk.Domain.Tickets.Enums;

namespace FlowDesk.Domain.Tickets.Services;

public class SlaService(TimeProvider timeProvider)
{
    public bool IsCurrentlyBreached(Ticket ticket) =>
        ticket.IsSlaBreached || 
        (ticket.SlaDeadlineAt.HasValue && timeProvider.GetUtcNow().UtcDateTime > ticket.SlaDeadlineAt.Value);

    public TimeSpan? TimeRemaining(Ticket ticket)
    {
        if (!ticket.SlaDeadlineAt.HasValue) return null;
        
        var remaining = ticket.SlaDeadlineAt.Value - timeProvider.GetUtcNow().UtcDateTime;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    public string GetRemainingTimeDisplay(Ticket ticket)
    {
        var remaining = TimeRemaining(ticket);
        if (remaining is null) return "No SLA";

        if(remaining.Value.TotalDays >= 1)
            return $"{remaining.Value.Days}d {remaining.Value.Hours}h";
        
        if (remaining.Value.TotalHours >= 1)
        return $"{remaining.Value.Hours}h {remaining.Value.Minutes}m";

        return $"{remaining.Value.Minutes}m";
    }


    public SlaRiskLevel GetRiskLevel(Ticket ticket)
    {
        if (!ticket.SlaDeadlineAt.HasValue) return SlaRiskLevel.NoSla;
        if (IsCurrentlyBreached(ticket)) return SlaRiskLevel.Breached;

        var total = (ticket.SlaDeadlineAt.Value - ticket.CreatedAt).TotalSeconds;
        if (total <= 0) return SlaRiskLevel.Critical;

        var remaining = TimeRemaining(ticket)?.TotalSeconds ?? 0;
        var percentage = (remaining / total) * 100.0;

        return percentage switch
        {
            > 50 => SlaRiskLevel.OnTrack,
            > 20 => SlaRiskLevel.AtRisk,
            _    => SlaRiskLevel.Critical
        };
    }

    public IEnumerable<Ticket> OrderBySlaPriority(IEnumerable<Ticket> tickets) =>
        tickets.OrderByDescending(GetRiskLevel)           
            .ThenBy(t => TimeRemaining(t) ?? TimeSpan.MaxValue);


    public SlaSummary GetSlaSummary(IEnumerable<Ticket> tickets)
    {
        var list = tickets.ToList();
        var groups = list.GroupBy(GetRiskLevel).ToDictionary(g => g.Key, g => g.Count());
        
        var resolved = list.Where(t => t.ResolvedAt.HasValue).ToList();
        var compliance = resolved.Count == 0 
            ? 100.0 
            : (double)resolved.Count(t => !t.IsSlaBreached) / resolved.Count * 100.0;

        return new SlaSummary(
            Total: list.Count,
            Breached: groups.GetValueOrDefault(SlaRiskLevel.Breached, 0),
            Critical: groups.GetValueOrDefault(SlaRiskLevel.Critical, 0),
            AtRisk: groups.GetValueOrDefault(SlaRiskLevel.AtRisk, 0),
            OnTrack: groups.GetValueOrDefault(SlaRiskLevel.OnTrack, 0),
            NoSla: groups.GetValueOrDefault(SlaRiskLevel.NoSla, 0),
            CompliancePercent: compliance
        );
    }
}

 