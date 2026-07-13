using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Domain.Workspaces.Events;

namespace FlowDesk.Domain.UnitTests.Workspaces;

public class WorkspaceTests
{
    private static Workspace CreateWorkspace(string name = "Engineering", string description = "desc")
        => Workspace.Create(name, description);
    
    [Fact]
    public void Create_ShouldSetPropertiesCorrectly()
    {
        var workspace = Workspace.Create("Engineering", "desc");

        Assert.Equal("Engineering", workspace.Name.Value);
        Assert.Equal("ENGINEERING", workspace.NormalizedName);
        Assert.False(workspace.IsDeleted);
        Assert.NotEqual(Guid.Empty, workspace.Id);
    }

    [Fact]
    public void AddMember_ShouldAddMember_WhenUserNotAlreadyMember()
    {
        var workspace = CreateWorkspace();
        var userId = Guid.NewGuid();

        workspace.AddMember(userId, WorkspaceRole.Member);

        Assert.Contains(workspace.WorkspaceMembers, m => m.UserId == userId);
    }

    [Fact]
    public void AddMember_ShouldThrow_WhenUserAlreadyMember()
    {
        var workspace = CreateWorkspace();
        var userId = Guid.NewGuid();
        workspace.AddMember(userId, WorkspaceRole.Member);

        Assert.Throws<DomainException>(() => workspace.AddMember(userId, WorkspaceRole.Admin));
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMember_AndRaiseDomainEvent()
    {
        var workspace = CreateWorkspace();
        var userId = Guid.NewGuid();
        workspace.AddMember(userId, WorkspaceRole.Member);

        workspace.RemoveMember(userId);

        Assert.DoesNotContain(workspace.WorkspaceMembers, m => m.UserId == userId);
        Assert.Contains(workspace.DomainEvents, e => e is WorkspaceMemberRemovedDomainEvent);
    }

    [Fact]
    public void RemoveMember_ShouldThrow_WhenMemberNotFound()
    {
        var workspace = CreateWorkspace();

        Assert.Throws<DomainException>(() => workspace.RemoveMember(Guid.NewGuid()));
    }

    [Fact]
    public void Delete_ShouldSetIsDeletedAndTimestamp()
    {
        var workspace = CreateWorkspace();

        workspace.Delete();

        Assert.True(workspace.IsDeleted);
        Assert.NotNull(workspace.DeletedAt);
    }

    [Fact]
    public void Restore_ShouldClearDeletedState()
    {
        var workspace = CreateWorkspace();
        workspace.Delete();

        workspace.Restore();

        Assert.False(workspace.IsDeleted);
        Assert.Null(workspace.DeletedAt);
        Assert.NotNull(workspace.RestoredAt);
    }
}

