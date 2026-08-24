using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery() : IRequest<AppUserDto?>;
