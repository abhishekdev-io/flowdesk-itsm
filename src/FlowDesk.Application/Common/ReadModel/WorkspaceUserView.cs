namespace FlowDesk.Application.Common.ReadModel;

public class WorkspaceUserView
{
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = null!;
}
