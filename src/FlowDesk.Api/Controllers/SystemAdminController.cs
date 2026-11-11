using FlowDesk.Application.Features.SystemAdmin.CreateSystemAdmin;
using FlowDesk.Application.Features.SystemAdmin.RestoreWorkspace;
using FlowDesk.Application.Features.SystemAdmin.ListDeletedWorkspaces;
using FlowDesk.Api.Dto.SystemAdmin;
using Asp.Versioning;

namespace FlowDesk.Api.Controllers;

[Authorize] 
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/systemadmin")]
public class SystemAdminController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterSystemAdminAsync(
        [FromBody] RegisterSystemAdminRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSystemAdminCommand(
            request.Email, 
            request.Username,
            request.AvatarUrl, 
            request.Password, 
            request.ConfirmPassword);
            
        var adminId = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new { Id = adminId });
    }

    [HttpPatch("workspaces/{workspaceId}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestoreWorkspaceAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var command = new RestoreWorkspaceCommand(workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }

    [HttpGet("workspaces/deleted")]
    [ProducesResponseType(typeof(List<DeletedWorkspaceSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListDeletedWorkspacesAsync(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListDeletedWorkspacesQuery(),
            cancellationToken);

        return Ok(result);
    }
}
