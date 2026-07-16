using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Behaviours;

internal sealed class SystemAdminBehaviour<TRequest, TResponse>(
    ICurrentUserContext currentUserContext
    ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ISystemAdminRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!currentUserContext.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserContext.IsSystemAdmin)
            throw new ForbiddenException("System admin access required.");

        return await next(cancellationToken);
    }
}