using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IAppUserRepository appUserRepository
) : IRequestHandler<GetCurrentUserQuery, AppUserDto?>
{
    public async Task<AppUserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user =  await appUserRepository.GetCurrentUserAsync(cancellationToken);

        return user ?? throw new NotFoundException("User", Guid.Empty);
    }
}
