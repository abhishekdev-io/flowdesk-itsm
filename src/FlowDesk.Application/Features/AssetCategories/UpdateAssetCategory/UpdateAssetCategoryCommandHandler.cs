using FlowDesk.Application.Interfaces;
using MediatR;
using FlowDesk.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.AssetCategories.UpdateAssetCategory;

public class UpdateAssetCategoryCommandHandler(
    IApplicationDbContext context)
        : IRequestHandler<UpdateAssetCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.WorkspaceId == request.WorkspaceId, cancellationToken)
                ?? throw new NotFoundException("Asset category", request.CategoryId);;

        var exists = await context.AssetCategories
            .AsNoTracking()
            .AnyAsync(c => c.WorkspaceId == request.WorkspaceId && c.Name.Value == request.Name 
                && c.Id != request.CategoryId, cancellationToken);

        if (exists)
            throw new ConflictException("Asset category name already exists in this workspace.");

        category.Update(request.Name, request.Prefix);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}



    

