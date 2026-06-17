using System.Security.Claims;
using FlowDesk.Application.Interface;
using Microsoft.AspNetCore.Http;

namespace FlowDesk.Infrastructure.Identity.Middleware;

public class CurrentUserMiddleware(RequestDelegate next)
{

    public async Task InvokeAsync(HttpContext context, ICurrentUserContext currentUser)
    {
        if(context.User.Identity?.IsAuthenticated == true)
        {
            var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid or missing user id in token");
            }

            currentUser.SetUserContext(
                userId,
                context.User.FindFirstValue(ClaimTypes.Email)!,
                true,
                context.User.HasClaim("is_system_admin", "true")
            );
        }
        
        await next(context);

    }
}
