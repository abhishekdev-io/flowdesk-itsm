using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.GetAssetCategory;

[RequirePermission(Permissions.CanViewCategories)]
public sealed record GetAssetCategoryByIdQuery(
    int CategoryId,
    Guid WorkspaceId
) : IRequest<AssetCategoryResponse>, IWorkspaceRequest;


