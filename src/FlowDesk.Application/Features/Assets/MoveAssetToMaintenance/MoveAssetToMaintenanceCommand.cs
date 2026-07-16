using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.MoveAssetToMaintenance;

[RequirePermission(Permissions.CanManageAssets)]
public sealed record MoveAssetToMaintenanceCommand(
    Guid AssetId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;
