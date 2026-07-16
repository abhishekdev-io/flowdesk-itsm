using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using MediatR;

namespace FlowDesk.Application.Features.Users.ListUsers;

public class ListUserQueryHandler(
    IAppUserRepository appUserRepository
) : IRequestHandler<ListUsersQuery, IEnumerable<AppUserDto>>
{
    public async Task<IEnumerable<AppUserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        return await appUserRepository.ListUsersAsync(cancellationToken);
    }
}
