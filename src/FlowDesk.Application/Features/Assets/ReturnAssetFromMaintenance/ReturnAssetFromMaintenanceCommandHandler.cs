using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Assets.ReturnAssetFromMaintenance;

public class ReturnAssetFromMaintenanceCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<ReturnAssetFromMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(ReturnAssetFromMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var asset = await context.Assets.FirstOrDefaultAsync(c => c.Id == request.AssetId
                         && c.WorkspaceId == request.WorkspaceId,cancellationToken)
                            ?? throw new NotFoundException("Asset.",request.AssetId);

        asset.ReturnFromMaintenance();

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }        
}
