using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using MediatR;

namespace FlowDesk.Application.Features.Auth.Login;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.Email, request.Password);
    }
}
