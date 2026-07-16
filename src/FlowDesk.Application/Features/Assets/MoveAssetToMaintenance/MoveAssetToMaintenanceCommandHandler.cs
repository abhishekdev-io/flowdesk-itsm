using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Assets.MoveAssetToMaintenance;

public class AssetToMaintenanceCommandHandler( 
    IApplicationDbContext context)
    : IRequestHandler<MoveAssetToMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(MoveAssetToMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var asset = await context.Assets.FirstOrDefaultAsync(c => c.Id == request.AssetId
                         && c.WorkspaceId == request.WorkspaceId,cancellationToken)
                            ?? throw new NotFoundException("Asset.",request.AssetId);

        asset.SendForMaintenance();

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }        
}
