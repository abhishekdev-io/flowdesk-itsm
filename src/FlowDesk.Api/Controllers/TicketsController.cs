using FlowDesk.Application.Features.Tickets.ListTickets;
using FlowDesk.Application.Features.Tickets.GetTicket;
using FlowDesk.Application.Features.Tickets.CreateTicket;
using FlowDesk.Application.Features.Tickets.UpdateTicket;
using FlowDesk.Application.Features.Tickets.ArchiveTicket;
using FlowDesk.Application.Features.Tickets.UnArchiveTicket;
using FlowDesk.Application.Features.Tickets.AssignTicket;
using FlowDesk.Application.Features.Tickets.UnAssignTicket;
using FlowDesk.Application.Features.Tickets.ChangePriority;
using FlowDesk.Application.Features.Tickets.CloseTicket;
using FlowDesk.Application.Features.Tickets.ReopenTicket;
using FlowDesk.Application.Features.Tickets.ResolveTicket;
using FlowDesk.Application.Features.Tickets.ChangeType;
using FlowDesk.Application.Features.Tickets.TicketComments.CreateComment;
using FlowDesk.Application.Features.Tickets.TicketComments.EditComment;
using FlowDesk.Application.Features.Tickets.TicketComments.GetCommentsByTicketId;
using FlowDesk.Application.Features.Tickets.TicketComments.GetDeletedCommentsByTicketId;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Api.Dto.Ticket;
using FlowDesk.Application.Features.Tickets.TicketComments.DeleteComment;
using FlowDesk.Application.Features.Tickets.TicketComments.RestoreComment;
using FlowDesk.Application.Features.Tickets.GetTicketActivities;
using FlowDesk.Application.Features.Tickets.GetSlaDashboard;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/workspaces/{workspaceId}/tickets")]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TicketSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListTicketsAsync(
        [FromRoute]Guid workspaceId,
        [FromQuery] TicketFilterParams? filters,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListTicketsQuery(workspaceId, filters),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{ticketId}")]
    [ProducesResponseType(typeof(TicketDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketByIdAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetTicketByIdQuery(ticketId, workspaceId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("activities")]
    [ProducesResponseType(typeof(TicketDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketActivitiesAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetTicketActivitiesQuery(ticketId,workspaceId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("sla-dashboard")]
    [ProducesResponseType(typeof(TicketDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSlaSummaryAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSlaDashBoardQuery(workspaceId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromBody]CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTicketCommand(
            workspaceId,
            request.Title,
            request.Description,
            request.AssetId,
            request.Type,
            request.Priority);

        var ticketId = await mediator.Send(command,cancellationToken);

        return Created($"/api/workspaces/{workspaceId}/tickets/{ticketId}", new { Id = ticketId });
    }

    [HttpPut("{ticketId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromBody]UpdateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTicketCommand(
            ticketId,
            workspaceId,
            request.Title,
            request.Description
            );

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{ticketId}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ArchiveTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var command = new ArchiveTicketCommand(ticketId, workspaceId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/unarchive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UnArchiveTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var command = new UnArchiveTicketCommand(ticketId, workspaceId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromBody]AssignTicketRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignTicketCommand(
            ticketId, 
            request.AssigneeId,
            workspaceId
            );

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/unassign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UnassignTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var command = new UnAssignTicketCommand(ticketId, workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/priority")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangePriorityAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromBody]PriorityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangePriorityCommand(
            ticketId,
            request.Priority,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/type")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeTypeAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromBody]TypeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeTypeCommand(
            ticketId,
            request.Type,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent(); 
    }

    [HttpPatch("{ticketId}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CloseTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CloseTicketCommand(
            ticketId,
            request.Reason,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/reopen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReopenTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var command = new ReopenTicketCommand(
            ticketId,
            workspaceId );

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{ticketId}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResolveTicketAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var command = new ResolveTicketCommand(
            ticketId,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

//Ticket Comments 

    [HttpGet("{ticketId}/comments")]
    [ProducesResponseType(typeof(PagedResult<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommentsByTicketIdAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var query = new GetCommentsByTicketIdQuery(ticketId, workspaceId);
        var comments = await mediator.Send(query, cancellationToken);
        return Ok(comments);
    }

    [HttpGet("{ticketId}/deleted-comments")]
    [ProducesResponseType(typeof(PagedResult<DeletedCommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeletedCommentsByTicketIdAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        CancellationToken cancellationToken)
    {
        var query = new GetDeletedCommentsByTicketIdQuery(ticketId, workspaceId);

        var comments = await mediator.Send(query, cancellationToken);
        return Ok(comments);
    }

    [HttpPost("{ticketId}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCommentAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromBody]CommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand(
            ticketId,
            workspaceId,
            request.Description);

        await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{ticketId}/comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditCommentAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromRoute]Guid commentId,
        [FromBody]CommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new EditCommentCommand(
            ticketId,
            commentId,
            request.Description,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();
    } 

    [HttpDelete("{ticketId}/comments/{commentId}")]   
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCommentAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromRoute]Guid commentId,
        CancellationToken cancellationToken) 
    {
        var command = new DeleteCommentCommand(
            ticketId,
            commentId,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();    
    }    

    [HttpPatch("{ticketId}/comments/{commentId}/restore")] 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]  
    public async Task<IActionResult> RestoreCommentAsync(
        [FromRoute]Guid workspaceId,
        [FromRoute]Guid ticketId,
        [FromRoute]Guid commentId,
        CancellationToken cancellationToken) 
    {
        var command = new RestoreCommentCommand(
            ticketId,
            commentId,
            workspaceId);

        await mediator.Send(command, cancellationToken);
        return NoContent();    
    } 
}



