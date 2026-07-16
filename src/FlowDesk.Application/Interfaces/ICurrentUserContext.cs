namespace FlowDesk.Application.Interfaces;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
    bool IsSystemAdmin { get; }
    void SetUserContext(Guid userId, string email, bool isAuthenticated, bool isSystemAdmin);
}
