using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Users.GetUser;

public class GetUserQueryHandler(
    IAppUserRepository appUserRepository
) : IRequestHandler<GetUserQuery, AppUserDto?>
{
    public async Task<AppUserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await appUserRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        return user ?? throw new NotFoundException("User", request.UserId);
    }   
}
