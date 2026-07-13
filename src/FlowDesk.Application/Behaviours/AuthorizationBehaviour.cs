using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Behaviours;

public sealed class AuthorizationBehaviour<TRequest, TResponse>(
    ICurrentUserContext currentUserContext,
    ICurrentWorkspaceContext workspaceContext,
    IApplicationDbContext context
    ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IWorkspaceRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var workspaceId = request.WorkspaceId;
        var userId = currentUserContext.UserId;

        var membership = await context.WorkspaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId, cancellationToken)
                ?? throw new ForbiddenException("User is not a member of the workspace.");

        workspaceContext.SetWorkspaceContext(workspaceId, membership.Role);

        return await next(cancellationToken);
    }
}
 