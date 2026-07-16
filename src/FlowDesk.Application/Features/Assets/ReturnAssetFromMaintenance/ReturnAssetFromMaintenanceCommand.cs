using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.ReturnAssetFromMaintenance;

[RequirePermission(Permissions.CanManageAssets)]
public sealed record ReturnAssetFromMaintenanceCommand(
    Guid AssetId,
    Guid WorkspaceId
) : IRequest<Unit>,IWorkspaceRequest;