using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Interfaces;

public interface IAppUserRepository
{
    Task<IEnumerable<AppUserDto>> ListUsersAsync(CancellationToken cancellationToken);
}
