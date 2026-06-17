using FlowDesk.Application.Common.Dto;
using MediatR;

namespace FlowDesk.Application.Features.Auth.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password
    ) : IRequest<AuthResponseDto>;
}