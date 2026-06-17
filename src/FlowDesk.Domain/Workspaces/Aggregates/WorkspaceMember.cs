using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Domain.Workspaces.Aggregates;

public sealed class WorkspaceMember
{
    public Guid UserId { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Workspace Workspace { get; private set; } = null!;
    public WorkspaceRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; } 
    public DateTime UpdateAt { get; private set; }  

    private WorkspaceMember() { }

    public static WorkspaceMember Create(Guid userId, Guid workspaceId, WorkspaceRole role)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty.");

        if (workspaceId == Guid.Empty)
            throw new DomainException("Workspace ID cannot be empty.");
    
        return new WorkspaceMember
        {
            UserId = userId,
            WorkspaceId = workspaceId,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void ChangeRole(WorkspaceRole newRole)
    {
        if (Role == newRole)
            return;

        Role = newRole;
        UpdateAt = DateTime.UtcNow;
    }
}


