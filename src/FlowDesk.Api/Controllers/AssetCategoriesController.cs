using FlowDesk.Application.Features.AssetCategories.ListAssetCategories;
using FlowDesk.Application.Features.AssetCategories.CreateAssetCategory;
using FlowDesk.Application.Features.AssetCategories.UpdateAssetCategory;
using FlowDesk.Application.Features.AssetCategories.DeactivateAssetCategory;
using FlowDesk.Application.Features.AssetCategories.GetAssetCategory;
using FlowDesk.Application.Features.AssetCategories.RestoreAssetCategory;
using FlowDesk.Application.Features.AssetCategories.ListDeactivatedAssetCategories;
using FlowDesk.Api.Dto.AssetCategory;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/workspaces/{workspaceId}/categories")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
[ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
public class AssetCategoriesController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AssetCategorySummaryResponse>))]
    public async Task<IActionResult> ListAssetCategoriesAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListAssetCategoriesQuery(workspaceId),cancellationToken);
        return Ok(result);
    }

    [HttpGet("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAssetCategoryByIdAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]int categoryId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAssetCategoryByIdQuery(categoryId, workspaceId),cancellationToken);
        return Ok(result);
    }   

    [HttpGet("deactivated")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DeactivatedAssetCategoryResponse>))]
    public async Task<IActionResult> ListDeactivatedAssetCategoriesAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListDeactivatedAssetCategoriesQuery(workspaceId),cancellationToken);
        return Ok(result);
    } 

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateAssetCategoryAsync(
        [FromRoute]Guid workspaceId, 
        [FromBody]AssetCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssetCategoryCommand(
            request.Name, 
            request.Prefix, 
            workspaceId);
    
        var categoryId = await mediator.Send(command,cancellationToken);

        return Created($"/api/workspaces/{workspaceId}/categories/{categoryId}", new { Id = categoryId });

    }

    [HttpPut("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateAssetCategoryAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]int categoryId,
        [FromBody]AssetCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAssetCategoryCommand(
            categoryId, 
            request.Name, 
            request.Prefix, 
            workspaceId);
            
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }

    [HttpPatch("{categoryId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeactivateAssetCategoryAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]int categoryId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateAssetCategoryCommand(categoryId, workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }

    [HttpPatch("{categoryId}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> RestoreAssetCategoryAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]int categoryId,
        CancellationToken cancellationToken)
    {
        var command = new RestoreAssetCategoryCommand(categoryId, workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }
}

