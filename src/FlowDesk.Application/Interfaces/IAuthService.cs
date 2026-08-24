using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(string userName, string email, string password, string? avatarUrl, CancellationToken ct);
    Task<Guid> RegisterSystemAdminAsync(string userName, string email, string password, string? avatarUrl, CancellationToken ct);
    Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken ct);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct);
}
