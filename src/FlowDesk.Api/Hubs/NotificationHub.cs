using System.Security.Claims;
using FlowDesk.Application.Interfaces.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace FlowDesk.Api.Hubs;

[Authorize]
public sealed class NotificationHub(IMembershipChecker membershipChecker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userIdValue = Context.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            Context.Abort();
            return;
        }
    
        var workspaceIdValue = Context.GetHttpContext()?
            .Request.Query["workspaceId"].ToString();

        if (!Guid.TryParse(workspaceIdValue, out var workspaceId))
        {
            Context.Abort();
            return;
        }

        var isMember = await membershipChecker.IsMemberAsync(workspaceId, userId, Context.ConnectionAborted);

        if (!isMember)
        {
            Context.Abort();
            return;
        }

        var groupName = $"workspace:{workspaceId}:user:{userId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await base.OnConnectedAsync();
    }
}
 