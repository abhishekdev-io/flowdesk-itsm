using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using FlowDesk.Domain.Assets;

namespace FlowDesk.Application.Features.Assets.CreateAsset;

public class CreateAssetCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<CreateAssetCommand, Guid>
{
    public async Task<Guid> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Assets
                .AsNoTracking()
                .AnyAsync(c => c.WorkspaceId == request.WorkspaceId && c.Name.Value == request.Name, cancellationToken);

        if (exists)
            throw new ConflictException($"Asset with name {request.Name} already exists in this workspace.");

        var category = await context.AssetCategories
               .FirstOrDefaultAsync(c => c.Id == request.CategoryId &&
                    c.WorkspaceId == request.WorkspaceId,
                    cancellationToken)
                    ?? throw new NotFoundException("AssetCategory", request.CategoryId);

        var assetTag = category.GenerateAssetTag();

        var asset = Asset.Create(
            request.Name,
            request.WorkspaceId,
            request.CategoryId,
            request.Modelnumber,
            assetTag,
            request.Serialnumber,
            request.Manufacturer
            );

        context.Assets.Add(asset);   

        try
        {
            // 4. Save safely. If someone else changed NextAssetNumber, this throws.
            await context.SaveChangesAsync(cancellationToken);
            return asset.Id;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // 5. Throw a clean exception mapped to an HTTP 409 Conflict in your API
            throw new ConflictException(
                $"The asset tag could not be generated because the category was updated by another user. Please try again - {ex}");
        }

    }
}
