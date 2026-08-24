using FlowDesk.Domain.Workspaces.Aggregates;
using Microsoft.AspNetCore.Identity;

namespace FlowDesk.Infrastructure.Identity.Entity;

public class AppUser : IdentityUser<Guid>
{
    public string? AvatarUrl {get; private set;}
    public bool IsSystemAdmin { get; private set; }
    private AppUser() { }
    public static AppUser Create(string username, string email, string? avatarUrl = null)
    {
        var user = new AppUser
        {
            UserName = username,
            Email = email,
            AvatarUrl = avatarUrl,
            IsSystemAdmin = false
        };
        return user;
    }

    public static AppUser CreateSystemAdmin(string email, string userName, string? avatarUrl = null)
    {
        return new AppUser
        {
            Email = email,
            UserName = userName,
            AvatarUrl = avatarUrl,
            IsSystemAdmin = true
        };
    }
    public void PromoteToSystemAdmin() => IsSystemAdmin = true;
    
    public ICollection<WorkspaceMember> WorkspaceMembers { get; private set; } = [];

}
