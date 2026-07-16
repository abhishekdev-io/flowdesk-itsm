using FlowDesk.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.Workspaces.ListWorkspaces;

public class ListWorkspacesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserContext user)
     : IRequestHandler<ListWorkspacesQuery, IEnumerable<WorkspaceSummaryResponse>>
{    public async Task<IEnumerable<WorkspaceSummaryResponse>> Handle(ListWorkspacesQuery request, CancellationToken cancellationToken)
    {
        return await context.Workspaces
                .AsNoTracking()
                .Where(w => w. WorkspaceMembers.Any(wm =>
                    wm.UserId == user.UserId))
                .OrderBy(w => w.Name.Value)
                .Select(w => new WorkspaceSummaryResponse
                (
                    w.Id,
                    w.Name.Value,
                    w.Description,
                    w.CreatedAt
                )).ToListAsync(cancellationToken);
    }
}
 