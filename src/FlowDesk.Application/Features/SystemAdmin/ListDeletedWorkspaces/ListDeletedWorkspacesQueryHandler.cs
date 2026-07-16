using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Workspaces.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.Features.SystemAdmin.ListDeletedWorkspaces;

public class ListDeletedWorkspacesQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<ListDeletedWorkspacesQuery, IEnumerable<DeletedWorkspaceSummaryResponse>>
{
    public async Task<IEnumerable<DeletedWorkspaceSummaryResponse>> Handle(ListDeletedWorkspacesQuery request, CancellationToken cancellationToken)
    {
        return await context.Workspaces
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(w => w.IsDeleted)
            .Select(w => new DeletedWorkspaceSummaryResponse(
                w.Id,
                w.Name.Value,
                w.Description,
                w.DeletedAt
            )).ToListAsync(cancellationToken);
    }
}

 
 