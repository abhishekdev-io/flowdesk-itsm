using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.GetAsset;

[RequirePermission(Permissions.CanViewAssets)]
public sealed record GetAssetByIdQuery(
    Guid AssetId,
    Guid WorkspaceId
) : IRequest<AssetResponse>, IWorkspaceRequest;
