using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.Interfaces;

public interface ICurrentWorkspaceContext
{
    Guid WorkspaceId { get; }
    WorkspaceRole Role { get; }
    bool IsAdmin { get; }
    bool IsMember { get; }
    bool IsAgent { get; }

    void SetWorkspaceContext(Guid workspaceId, WorkspaceRole role);
}
