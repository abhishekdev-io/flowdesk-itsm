using FlowDesk.Api.Dto.Asset;
using FlowDesk.Application.Features.Assets.CreateAsset;
using FlowDesk.Application.Features.Assets.ListAssets;
using FlowDesk.Application.Features.Assets.GetAsset;
using FlowDesk.Application.Features.Assets.DecommissionAsset;
using FlowDesk.Application.Features.Assets.RestoreAsset;
using FlowDesk.Application.Features.Assets.MoveAssetToMaintenance;
using FlowDesk.Application.Features.Assets.ReturnAssetFromMaintenance;
using FlowDesk.Application.Features.Assets.UpdateAsset;
using FlowDesk.Application.Common.Pagination;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/workspaces/{workspaceId}/assets")]
public class AssetsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AssetSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListAssetsAsync(
        [FromRoute]Guid workspaceId,
        [FromQuery] AssetFilterParams? filters,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListAssetsQuery(workspaceId, filters),
            cancellationToken);

        return Ok(result);
    } 

    [HttpGet("{assetId}")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssetByIdAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid assetId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAssetByIdQuery(assetId, workspaceId),
            cancellationToken);
            
        return Ok(result);
    }

    [HttpPost]  
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAssetAsync(
        [FromRoute]Guid workspaceId,
        [FromBody]AssetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssetCommand(
            request.Name,
            request.CategoryId,
            request.Modelnumber,
            request.Serialnumber,
            request.Manufacturer,
            workspaceId);

        var assetId = await mediator.Send(command,cancellationToken);
        
        return Created($"/api/workspaces/{workspaceId}/assets/{assetId}", new { Id = assetId });

    } 

    [HttpPut("{assetId}")]  
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAssetAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid assetId,
        [FromBody]AssetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAssetCommand(
            request.Name,
            assetId,
            request.CategoryId,
            request.Modelnumber,
            request.Manufacturer,
            request.Serialnumber,
            workspaceId);
        
        await mediator.Send(command,cancellationToken);
        return NoContent();
    } 

    [HttpPatch("{assetId}/decommission")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DecommissionAssetAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid assetId,
        CancellationToken cancellationToken)
    {
        var command = new DecommissionAssetCommand(assetId,workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }    

    [HttpPatch("{assetId}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestoreAssetAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid assetId,
        CancellationToken cancellationToken)
    {
        var command = new RestoreAssetCommand(assetId,workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }        

    [HttpPatch("{assetId}/move-to-maintenance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MoveAssetToMaintenanceAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid assetId,
        CancellationToken cancellationToken)
    {
        var command = new MoveAssetToMaintenanceCommand(assetId,workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    } 

    [HttpPatch("{assetId}/return-from-maintenance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReturnAssetFromMaintenanceAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid assetId,
        CancellationToken cancellationToken)
    {
        var command = new ReturnAssetFromMaintenanceCommand(assetId,workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    } 
}

