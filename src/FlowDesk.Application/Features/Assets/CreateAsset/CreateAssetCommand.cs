using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;

namespace FlowDesk.Application.Features.Assets.CreateAsset;

[RequirePermission(Permissions.CanManageAssets)]
public sealed record CreateAssetCommand(
    string Name,
    int CategoryId,
    string Modelnumber,
    string? Serialnumber,
    string Manufacturer,
    Guid WorkspaceId
) : IRequest<Guid>,IWorkspaceRequest;

