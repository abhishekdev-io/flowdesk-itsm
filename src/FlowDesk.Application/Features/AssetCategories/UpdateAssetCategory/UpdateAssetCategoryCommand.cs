using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.UpdateAssetCategory;

[RequirePermission(Permissions.CanManageCategories)]
public sealed record UpdateAssetCategoryCommand(
    int CategoryId,
    string Name,
    string Prefix,
    Guid WorkspaceId
) : IRequest<Unit>, IWorkspaceRequest;


