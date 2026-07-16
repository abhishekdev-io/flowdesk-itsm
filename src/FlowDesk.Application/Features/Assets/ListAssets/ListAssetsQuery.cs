using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Assets;

namespace FlowDesk.Application.Features.Assets.ListAssets;

[RequirePermission(Permissions.CanViewAssets)]
public sealed record ListAssetsQuery(
    Guid WorkspaceId,
    AssetFilterParams? Filters = null
) : IRequest<PagedResult<AssetSummaryResponse>>, IWorkspaceRequest;

public record AssetFilterParams : QueryParameters
{
    public AssetStatus? Status { get; init; }
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
};
