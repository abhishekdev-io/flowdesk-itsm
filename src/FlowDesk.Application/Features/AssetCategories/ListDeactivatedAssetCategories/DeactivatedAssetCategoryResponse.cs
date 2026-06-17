namespace FlowDesk.Application.Features.AssetCategories.ListDeactivatedAssetCategories;

public record DeactivatedAssetCategoryResponse(
    int Id,
    string Name,
    string Prefix,
    DateTime? DeactivatedAt
);

