using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Users.GetUser;

public record GetUserQuery(
    Guid UserId
) : IRequest<AppUserDto>;
