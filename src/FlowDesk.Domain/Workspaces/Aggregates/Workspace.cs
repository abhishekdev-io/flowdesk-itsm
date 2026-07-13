using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Domain.Workspaces.ValueObjects;
using FlowDesk.Domain.Workspaces.Events;

namespace FlowDesk.Domain.Workspaces.Aggregates;

public sealed class Workspace : AggregateRoot<Guid>
{
    public WorkspaceName Name {get; private set;} = null!;
    public string NormalizedName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime? RestoredAt { get; private set; }

    private readonly List<WorkspaceMember> _workspaceMembers = [];
    public IReadOnlyCollection<WorkspaceMember> WorkspaceMembers => _workspaceMembers;
 
    private Workspace() : base(Guid.Empty) { }

    public void Delete()
    {
        if(IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        RestoredAt = DateTime.UtcNow;
    }
    //Create Workspace
    public static Workspace Create(string name, string description)
    {
        var workspaceName = WorkspaceName.Create(name); 
        return new Workspace
        {
            Id = Guid.NewGuid(),
            Name = workspaceName,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            NormalizedName = workspaceName.Value.Trim().ToUpperInvariant(),
            IsDeleted = false
        };
    }
    //Update Workspace
    public void Update(string name, string description)
    {
        Name = WorkspaceName.Create(name);
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        NormalizedName = Name.Value.Trim().ToUpperInvariant();
    }

    //Add member
    public void AddMember(Guid userId, WorkspaceRole role)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Invalid user.");

        if (_workspaceMembers.Any(m => m.UserId == userId))
            throw new DomainException("User is already a member of the workspace.");            

        var member = WorkspaceMember.Create(userId,Id, role);
        _workspaceMembers.Add(member);
    }
    //Remove member
    public void RemoveMember(Guid userId)
    {
        var member = _workspaceMembers.FirstOrDefault(m => m.UserId == userId)
            ?? throw new DomainException("Member not found.");

        _workspaceMembers.Remove(member);

        RaiseDomainEvent(new WorkspaceMemberRemovedDomainEvent(
            WorkspaceId: Id,
            RemovedUserId: userId,
            WorkspaceName: Name
        ));
    }
    //Change member role
    public void ChangeMemberRole(Guid userId, WorkspaceRole newRole)
    {
        var member = _workspaceMembers.FirstOrDefault(m => m.UserId == userId)
            ?? throw new DomainException("Member not found.");

        member.ChangeRole(newRole);

        RaiseDomainEvent(new WorkspaceRoleChangedDomainEvent(
            WorkspaceId: Id,
            RoleChangedUserId: userId,
            CurrentRole: newRole,
            WorkspaceName: Name
        ));

    }
}

