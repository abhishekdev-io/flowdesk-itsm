using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.Workspaces.GetWorkspace;

public class GetWorkspaceCommandHandler(
    IApplicationDbContext context,
    ICurrentUserContext user
) : IRequestHandler<GetWorkspaceByIdQuery,WorkspaceResponse>
{
    public async Task<WorkspaceResponse> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Workspaces
                .AsNoTracking()
                .Where(w => w.Id == request.WorkspaceId && 
                     w.WorkspaceMembers.Any(wm => wm.UserId == user.UserId))
                .Select(w => new WorkspaceResponse(
                    w.Id,
                    w.Name.Value,
                    w.Description,
                    w.CreatedAt
                )).FirstOrDefaultAsync(cancellationToken)
                     ?? throw new NotFoundException("workspace",request.WorkspaceId);
    }
}
