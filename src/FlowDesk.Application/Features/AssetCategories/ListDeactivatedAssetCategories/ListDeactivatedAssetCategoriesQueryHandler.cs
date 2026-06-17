using FlowDesk.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.AssetCategories.ListDeactivatedAssetCategories;

public class ListDeactivatedAssetCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListDeactivatedAssetCategoriesQuery,IEnumerable<DeactivatedAssetCategoryResponse>>
{
    public async Task<IEnumerable<DeactivatedAssetCategoryResponse>> Handle(ListDeactivatedAssetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await context.AssetCategories
            .AsNoTracking()
            .Where(c => !c.IsActive)
            .OrderByDescending(c => c.DeactivatedAt)
            .Select(c => new DeactivatedAssetCategoryResponse(
                c.Id,
                c.Name.Value,
                c.Prefix,
                c.DeactivatedAt
            ))
            .ToListAsync(cancellationToken);
        
    }
}