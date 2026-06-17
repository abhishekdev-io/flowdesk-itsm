using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Exceptions;

namespace FlowDesk.Application.Features.Assets.GetAsset;

public class GetAssetByIdQueryHandler(
    IApplicationDbContext context) : IRequestHandler<GetAssetByIdQuery, AssetResponse>
{
    public async Task<AssetResponse> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await context.Assets
            .AsNoTracking()
            .Where(a => a.Id == request.AssetId && a.WorkspaceId == request.WorkspaceId)
            .Select(a => new AssetResponse
            (
                a.Id,
                a.Name.Value,
                a.AssetTag,
                a.Status.ToString(),
                a.AssetCategory.Name.Value,
                a.Manufacturer,
                a.ModelNumber,
                a.SerialNumber,
                a.WorkspaceId
            )).FirstOrDefaultAsync(cancellationToken);

        return asset ?? throw new NotFoundException("Asset", request.AssetId);
    }
}
