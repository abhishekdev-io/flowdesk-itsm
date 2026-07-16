using FlowDesk.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.AssetCategories.ListAssetCategories;

public class ListAssetCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListAssetCategoriesQuery,IEnumerable<AssetCategorySummaryResponse>>
{
    public async Task<IEnumerable<AssetCategorySummaryResponse>> Handle(ListAssetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await context.AssetCategories
        .AsNoTracking()
        .Where(c => c.WorkspaceId == request.WorkspaceId && c.IsActive)
        .OrderBy(c => c.Name.Value)
        .Select(c => new AssetCategorySummaryResponse
        (
            c.Id,
            c.Name.Value,
            c.Prefix,
            c.WorkspaceId
        )).ToListAsync(cancellationToken);
        
    }
}

    

