using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Pagination;

namespace FlowDesk.Application.Features.Assets.ListAssets;

public class ListAssetQueryHandler(
    IApplicationDbContext context) 
    : IRequestHandler<ListAssetsQuery, PagedResult<AssetSummaryResponse>>
{
    public async Task<PagedResult<AssetSummaryResponse>> Handle(ListAssetsQuery request, CancellationToken cancellationToken)
    {

        var query = context.Assets
            .AsNoTracking()
            .Where(a => a.WorkspaceId == request.WorkspaceId);

        var f = request.Filters ?? new AssetFilterParams();

        if (f.Status.HasValue)
        {
            query = query.Where(a => a.Status == f.Status);
        }
        if (f.CategoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == f.CategoryId.Value);
        }            
        if(!string.IsNullOrWhiteSpace(f.Search))
        {
            var search = f.Search.Trim();

            query = query.Where(a =>
                a.Name.Value.Contains(search) ||
                a.AssetCategory.Name.Value.Contains(search) ||
                a.AssetTag.Contains(search)
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = f.SortBy?.ToLower() switch
        {
            "createdat" => f.Descending ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt),

            "category" => f.Descending ? query.OrderByDescending(a => a.AssetCategory.Name.Value)
                : query.OrderBy(a => a.AssetCategory.Name.Value),

            "status" => f.Descending ? query.OrderByDescending(a => a.Status)
                : query.OrderBy(a => a.Status),

            "name" => f.Descending ? query.OrderByDescending(a => a.Name.Value)
                : query.OrderBy(a => a.Name.Value),

            "manufacturer" => f.Descending ? query.OrderByDescending(a => a.Manufacturer)
                : query.OrderBy(a => a.Manufacturer),

             _ => query.OrderByDescending(a => a.CreatedAt)    
        };

        var assets = await query
            .Skip((f.Page - 1) * f.PageSize)
            .Take(f.PageSize)
            .Select(a => new AssetSummaryResponse
            (
                a.Id,
                a.Name.Value,
                a.AssetTag,
                a.Status,
                a.AssetCategory.Name.Value,
                a.Manufacturer,
                a.CreatedAt,
                a.DecommissionedAt
            )).ToListAsync(cancellationToken);

        return new PagedResult<AssetSummaryResponse>
        {
            Items = assets,
            Page = f.Page,
            PageSize = f.PageSize,
            TotalCount = totalCount
        };
    }
}
