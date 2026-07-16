using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.RestoreAsset;

[RequirePermission(Permissions.CanManageAssets)]
public sealed record RestoreAssetCommand(
    Guid AssetId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;
