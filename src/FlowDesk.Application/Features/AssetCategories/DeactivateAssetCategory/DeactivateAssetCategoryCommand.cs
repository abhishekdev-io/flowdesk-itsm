using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.DeactivateAssetCategory;

[RequirePermission(Permissions.CanManageCategories)]
public sealed record DeactivateAssetCategoryCommand(
    int CategoryId,
    Guid WorkspaceId
) : IRequest<Unit>, IWorkspaceRequest;
