using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResponseDto>;
