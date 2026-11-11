using FlowDesk.Application.Features.Workspaces.ListWorkspaces;
using FlowDesk.Application.Features.Workspaces.GetWorkspace;
using FlowDesk.Api.Dto.Workspace;
using FlowDesk.Application.Features.Workspaces.CreateWorkspace;
using FlowDesk.Application.Features.Workspaces.UpdateWorkspace;
using FlowDesk.Application.Features.Workspaces.DeleteWorkspace;
using FlowDesk.Application.Features.Workspaces.WorkspaceMember.AddMember;
using FlowDesk.Application.Features.Workspaces.WorkspaceMember.RemoveMember;
using FlowDesk.Application.Features.Workspaces.WorkspaceMember.GetMemberById;
using FlowDesk.Application.Features.Workspaces.WorkspaceMember.ListMembers;
using FlowDesk.Application.Features.Workspaces.WorkspaceMember.ChangeMemberRole;
using FlowDesk.Application.Common.Dto;
using Asp.Versioning;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/workspaces")]
public class WorkspacesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<WorkspaceSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListWorkspacesAsync(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListWorkspacesQuery(),
            cancellationToken);   

        return Ok(result);
    }

    [HttpGet("{workspaceId:guid}")]
    [ProducesResponseType(typeof(WorkspaceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkspaceByIdAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetWorkspaceByIdQuery(workspaceId),
            cancellationToken);
            
        return Ok(result);
    } 

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateWorkspaceAsync(
        [FromBody]WorkspaceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkspaceCommand(
            request.Name,
            request.Description);
            
        var workspaceId = await mediator.Send(command,cancellationToken);

        return Created($"/api/workspaces/{workspaceId}", new { Id = workspaceId });
    }    

    [HttpPut("{workspaceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkspaceAsync(
        [FromRoute]Guid workspaceId,
        [FromBody]WorkspaceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWorkspaceCommand(
            workspaceId,
            request.Name,
            request.Description);

        await mediator.Send(command,cancellationToken);
        return NoContent();
    } 

    [HttpDelete("{workspaceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkspaceAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWorkspaceCommand(workspaceId);
        await mediator.Send(command,cancellationToken);
        return NoContent();
    }    


//WorkspaceMember

    [HttpGet("{workspaceId}/members/{userId}")]
    [ProducesResponseType(typeof(WorkspaceMemberDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMemberByIdAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMemberByIdQuery(userId, workspaceId),
            cancellationToken);
            
        return Ok(result);
    }

    [HttpGet("{workspaceId}/members")]
    [ProducesResponseType(typeof(List<WorkspaceMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListMembersAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListMembersQuery(workspaceId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{workspaceId}/members")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMemberAsync( 
        [FromRoute]Guid workspaceId,
        [FromBody]AddMemberRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddMemberCommand(
            request.UserId,
            workspaceId,
            request.Role);

        await mediator.Send(command,cancellationToken);
        
        return StatusCode(StatusCodes.Status201Created);  
    }

    [HttpPatch("{workspaceId}/members/{userId}/change-role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeMemberRoleAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid userId,
        [FromBody]ChangeMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeMemberRoleCommand(
            userId,
            workspaceId,
            request.Role);

        await mediator.Send(command,cancellationToken);
        return NoContent();
    }

    [HttpDelete("{workspaceId}/members/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMemberAsync( 
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMemberCommand(
            userId,
            workspaceId);

        await mediator.Send(command,cancellationToken);
        return NoContent();    
    }
}
