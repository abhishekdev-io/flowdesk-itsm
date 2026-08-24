namespace FlowDesk.Application.Features.Assets.AssetStats;

public record AssetStatsResponse(
    int Total,
    int Active,
    int Maintenance,
    int Decommissioned
);
