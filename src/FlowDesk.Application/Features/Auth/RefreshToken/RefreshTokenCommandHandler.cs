using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.Auth.RefreshToken;

public class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await authService.RefreshTokenAsync(
            request.RefreshToken,
            cancellationToken
            );
    }
}