using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Assets.DecommissionAsset;

public class DecommissionAssetCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DecommissionAssetCommand, Unit>
{
    public async Task<Unit> Handle(DecommissionAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await context.Assets
                .FirstOrDefaultAsync(c => c.Id == request.AssetId
                    && c.WorkspaceId == request.WorkspaceId,cancellationToken)
                    ?? throw new NotFoundException("Asset.",request.AssetId);

        asset.Decommission();

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }        
}

