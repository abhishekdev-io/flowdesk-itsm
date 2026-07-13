namespace FlowDesk.Application.Features.Assets.GetAsset;

public record AssetResponse(
    Guid Id,
    string Name,
    string AssetTag,
    string Status,
    string Category,
    string Manufacturer,
    string Modelnumber,
    string? Serialnumber,
    Guid WorkspaceId
);


 