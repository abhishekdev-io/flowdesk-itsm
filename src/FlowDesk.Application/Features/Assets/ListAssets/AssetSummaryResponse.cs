using FlowDesk.Domain.Assets;

namespace FlowDesk.Application.Features.Assets.ListAssets;

public record AssetSummaryResponse(
    Guid Id,
    string Name,
    string AssetTag,
    AssetStatus Status,   
    string CategoryName,
    string Manufacturer,
    DateTime CreatedAt,
    DateTime? DecommissionedAt
);
