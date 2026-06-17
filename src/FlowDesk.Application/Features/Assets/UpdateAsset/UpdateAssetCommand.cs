using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.UpdateAsset;

[RequirePermission(Permissions.CanManageAssets)]
public sealed record UpdateAssetCommand(
    string Name,
    Guid AssetId,
    int CategoryId,
    string Modelnumber,
    string Manufacturer,
    string? Serialnumber,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;
