using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.RestoreAssetCategory;

[RequirePermission(Permissions.CanManageCategories)]
public sealed record RestoreAssetCategoryCommand(
    int CategoryId,
    Guid WorkspaceId
) : IRequest<Unit>, IWorkspaceRequest;
