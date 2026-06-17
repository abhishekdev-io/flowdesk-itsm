using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Interface;

public interface IAppUserRepository
{
    Task<IEnumerable<AppUserDto>> ListUsersAsync(CancellationToken cancellationToken);
}
