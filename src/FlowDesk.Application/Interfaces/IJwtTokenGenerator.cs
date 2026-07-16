namespace FlowDesk.Application.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateToken(Guid userId, string email, string userName);
    (string RawToken, string TokenHash) GenerateRefreshToken();
    string HashToken(string rawToken);
}
