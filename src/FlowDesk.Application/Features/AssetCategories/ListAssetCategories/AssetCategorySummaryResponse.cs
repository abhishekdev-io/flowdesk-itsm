namespace FlowDesk.Application.Features.AssetCategories.ListAssetCategories;

public record AssetCategorySummaryResponse(
    int Id,
    string Name,
    string Prefix,
    Guid WorkspaceId
);

