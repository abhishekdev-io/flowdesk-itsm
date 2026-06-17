using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Assets.UpdateAsset;

public class UpdateAssetCommandHandler(
    IApplicationDbContext context)
        : IRequestHandler<UpdateAssetCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AssetId && a.WorkspaceId == request.WorkspaceId, cancellationToken)
                ?? throw new NotFoundException("asset", request.AssetId);;

        var exists = await context.Assets
            .AsNoTracking()
            .AnyAsync(c => c.WorkspaceId == request.WorkspaceId && c.Name.Value == request.Name 
                && c.Id != request.AssetId, cancellationToken);

        if (exists)
            throw new ConflictException($"Asset name {request.Name} already exists in this workspace.");

        asset.Update(
            request.Name,
            request.CategoryId,
            request.Modelnumber,
            request.Manufacturer,
            request.Serialnumber);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
