using FlowDesk.Application.Features.Users.ListUsers;
using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
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
}
