using FlowDesk.Application.Interface;
using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.Common.Context;

public class CurrentWorkspaceContext : ICurrentWorkspaceContext
{
    public Guid WorkspaceId { get; private set; }
    public WorkspaceRole Role { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsMember { get; private set; }
    public bool IsAgent { get; private set; }

    public void SetWorkspaceContext(Guid workspaceId, WorkspaceRole role)
    {
        WorkspaceId = workspaceId;
        Role = role;
        IsAdmin = role == WorkspaceRole.Admin;
        IsMember = role == WorkspaceRole.Member;
        IsAgent = role == WorkspaceRole.Agent;
    }
}

