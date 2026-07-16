using FlowDesk.Application.Interfaces;

namespace FlowDesk.Infrastructure.Identity.Context;

public class CurrentUserContext : ICurrentUserContext
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public bool IsAuthenticated { get; private set; }
    public bool IsSystemAdmin { get; private set; }

    public void SetUserContext(Guid userId, string email, bool isAuthenticated, bool isSystemAdmin)
    {
        UserId = userId;
        Email = email;
        IsAuthenticated = isAuthenticated;
        IsSystemAdmin = isSystemAdmin;
    }
}

