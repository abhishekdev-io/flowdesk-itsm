using FlowDesk.Application.Interface;
using FlowDesk.Domain.Tickets.Services;

namespace FlowDesk.Application.Features.Tickets.GetSlaDashboard;

public sealed class GetSlaDashboardQueryHandler(
    IApplicationDbContext context,
    SlaService slaService
) : IRequestHandler<GetSlaDashBoardQuery, SlaDashboardDto>
{
    public async Task<SlaDashboardDto> Handle(GetSlaDashBoardQuery request, CancellationToken cancellationToken)
    {
        var tickets = await context.Tickets
            .Where(t => t.WorkspaceId == request.WorkspaceId)
            .ToListAsync(cancellationToken);

        var SlaSummary = slaService.GetSlaSummary(tickets);

        return new SlaDashboardDto(
            SlaSummary.Total,
            SlaSummary.Breached,
            SlaSummary.Critical,
            SlaSummary.AtRisk,
            SlaSummary.OnTrack,
            SlaSummary.NoSla,
            SlaSummary.CompliancePercent
        );
    }
}
