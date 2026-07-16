using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.AssetCategories.RestoreAssetCategory;

public class RestoreAssetCategoryCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<RestoreAssetCategoryCommand, Unit>
{
    public async Task<Unit> Handle(RestoreAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId
                && c.WorkspaceId == request.WorkspaceId,cancellationToken)
                    ?? throw new NotFoundException("Asset category.",request.CategoryId);

        category.Restore();

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }   

}
