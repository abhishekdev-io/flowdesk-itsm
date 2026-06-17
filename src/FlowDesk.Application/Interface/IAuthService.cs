using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(string userName, string email, string password, string avatarUrl);
    Task<Guid> RegisterSystemAdminAsync(string userName, string email, string password, string avatarUrl);
    Task<AuthResponseDto> LoginAsync(string email, string password);
}
