using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.ListDeactivatedAssetCategories;

[RequirePermission(Permissions.CanManageCategories)]
public sealed record ListDeactivatedAssetCategoriesQuery(
    Guid WorkspaceId
) : IRequest<IEnumerable<DeactivatedAssetCategoryResponse>>,IWorkspaceRequest;
