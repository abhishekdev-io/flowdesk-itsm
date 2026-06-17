namespace FlowDesk.Api.Dto.Asset;

public sealed record AssetRequest(
    string Name,
    int CategoryId,
    string Modelnumber,
    string Manufacturer,
    string? Serialnumber
);

