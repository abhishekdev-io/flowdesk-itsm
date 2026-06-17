using FlowDesk.Application.Interface;
using MediatR;
using FlowDesk.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.AssetCategories.DeactivateAssetCategory;

public class DeactivateAssetCategorydCommandHandler( 
    IApplicationDbContext context)
    : IRequestHandler<DeactivateAssetCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId
                && c.WorkspaceId == request.WorkspaceId,cancellationToken)
                    ?? throw new NotFoundException("Category.",request.CategoryId);

        category.Deactivate();

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }        
}

