using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Interfaces.Realtime;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Repository;

public class MembershipChecker(ApplicationDbContext context) : IMembershipChecker
{
    public Task<bool> IsMemberAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken)
    {

        return context.WorkspaceMembers
            .AsNoTracking()
            .AnyAsync(
                m => m.WorkspaceId == workspaceId &&
                     m.UserId == userId,
                cancellationToken);
    }
}
