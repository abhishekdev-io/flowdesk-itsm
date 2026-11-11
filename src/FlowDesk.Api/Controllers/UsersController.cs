using FlowDesk.Application.Features.Users.ListUsers;
using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Features.Users.GetUser;
using FlowDesk.Application.Features.Users.GetCurrentUser;
using Asp.Versioning;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsersAsync(
        [FromHeader(Name = "X-Workspace-Id")] Guid workspaceId, 
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new ListUsersQuery(workspaceId), 
            cancellationToken)
            );
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetUserQuery(userId),
            cancellationToken)
            );
    }

    [HttpGet("current")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUserAsync(
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetCurrentUserQuery(),
            cancellationToken)
            );
    }
}
