namespace FlowDesk.Application.Features.AssetCategories.GetAssetCategory;

public record AssetCategoryResponse(
    int Id,
    string Name,
    string Prefix,
    Guid WorkspaceId
);

