using FlowDesk.Application.Interfaces;
using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Auth.Register;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(
            request.UserName,
            request.Email,
            request.Password,
            request.AvatarUrl
            );
    }
}





