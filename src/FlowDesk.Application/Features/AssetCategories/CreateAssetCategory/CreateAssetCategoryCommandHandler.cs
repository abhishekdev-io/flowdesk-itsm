using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FlowDesk.Domain.AssetCategories;

namespace FlowDesk.Application.Features.AssetCategories.CreateAssetCategory;

public class CreateAssetCategoryCommandHandler( 
    IApplicationDbContext context)
    : IRequestHandler<CreateAssetCategoryCommand, int>
{    public async Task<int> Handle(CreateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
       var exists = await context.AssetCategories
            .AsNoTracking()
            .AnyAsync(c => c.WorkspaceId == request.WorkspaceId
                && c.Name.Value == request.Name, cancellationToken);

        if (exists)
            throw new ConflictException("Category name already exists in this workspace.");

        var category = AssetCategory.Create(
            request.Name,
            request.WorkspaceId,
            request.Prefix
            );

        await context.AssetCategories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}


