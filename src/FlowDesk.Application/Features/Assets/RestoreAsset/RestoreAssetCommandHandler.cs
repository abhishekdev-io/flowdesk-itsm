using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Assets.RestoreAsset
{
    public class RestoreAssetCommandHandler(
        IApplicationDbContext context
    ) : IRequestHandler<RestoreAssetCommand, Unit>
    {
        public async Task<Unit> Handle(RestoreAssetCommand request, CancellationToken cancellationToken)
        {
            var asset = await context.Assets
                .FirstOrDefaultAsync(a => a.Id == request.AssetId
                    && a.WorkspaceId == request.WorkspaceId,cancellationToken)
                    ?? throw new NotFoundException("Asset.",request.AssetId);

            asset.Restore();

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }   
    }
}