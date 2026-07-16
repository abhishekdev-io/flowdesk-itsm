using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.DecommissionAsset;

[RequirePermission(Permissions.CanManageAssets)]
public sealed record DecommissionAssetCommand(
    Guid AssetId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;

