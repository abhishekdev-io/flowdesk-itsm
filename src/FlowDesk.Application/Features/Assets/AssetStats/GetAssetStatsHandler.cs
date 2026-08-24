using FlowDesk.Application.Common.Permission;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Assets;

namespace FlowDesk.Application.Features.Assets.AssetStats;

[RequirePermission(Permissions.CanViewAssets)]
public sealed record GetAssetStatsQuery(Guid WorkspaceId) : IRequest<AssetStatsResponse>, IWorkspaceRequest;

public class GetAssetStatsHandler(
    IApplicationDbContext context
) : IRequestHandler<GetAssetStatsQuery, AssetStatsResponse>
{
    public async Task<AssetStatsResponse> Handle(GetAssetStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await context.Assets
        .Where(a => a.WorkspaceId == request.WorkspaceId)
        .GroupBy(a => 1)
        .Select(g => new AssetStatsResponse
        (
            g.Count(),
            g.Count(a => a.Status == AssetStatus.Active),
            g.Count(a => a.Status == AssetStatus.UnderMaintenance),
            g.Count(a => a.Status == AssetStatus.Decommissioned)
        ))
        .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new AssetStatsResponse(0, 0, 0, 0);
    }
}