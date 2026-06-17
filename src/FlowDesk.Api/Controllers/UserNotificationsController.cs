using FlowDesk.Application.Features.Notifications.GetUserNotifications;
using FlowDesk.Application.Features.Notifications.MarkAsRead;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Api.Dto.Notification;
using FlowDesk.Application.Features.Notifications.DeleteNotification;

namespace FlowDesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/workspaces/{workspaceId}/notifications")]
public class UserNotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserNotificationsAsync(
        [FromRoute]Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUserNotificationsQuery(workspaceId),
            cancellationToken);

        return Ok(result);
    }
    
    [HttpPatch("mark-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkMultipleReadAsync(
        Guid workspaceId,
        [FromBody] MarkAsReadRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkAsReadCommand(
            workspaceId,
            request.NotificationIds),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("delete-multiple")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMultipleAsync(
        Guid workspaceId,
        [FromBody] DeleteRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteMultipleNotificationsCommand(
            workspaceId,
            request.NotificationIds),
            cancellationToken);

        return NoContent();
    }
}
