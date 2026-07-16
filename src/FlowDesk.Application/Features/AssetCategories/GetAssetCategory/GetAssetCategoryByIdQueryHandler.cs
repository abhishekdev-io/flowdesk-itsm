using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.AssetCategories.GetAssetCategory;

public class GetAssetCategoryByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAssetCategoryByIdQuery, AssetCategoryResponse>
{
    public async Task<AssetCategoryResponse> Handle(GetAssetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.AssetCategories
        .AsNoTracking()
        .Where(c => c.Id == request.CategoryId && c.WorkspaceId == request.WorkspaceId)
        .Select(c => new AssetCategoryResponse
        (
            c.Id,
            c.Name.Value,
            c.Prefix,
            c.WorkspaceId
        )).FirstOrDefaultAsync(cancellationToken)
         ?? throw new NotFoundException("AssetCategory", request.CategoryId);
         
        return category;
    }
}
