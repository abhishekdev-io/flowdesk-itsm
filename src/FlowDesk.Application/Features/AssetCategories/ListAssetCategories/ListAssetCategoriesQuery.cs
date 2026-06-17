using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.ListAssetCategories;

[RequirePermission(Permissions.CanViewCategories)]
public sealed record ListAssetCategoriesQuery(
    Guid WorkspaceId
) : IRequest<IEnumerable<AssetCategorySummaryResponse>>,IWorkspaceRequest;


