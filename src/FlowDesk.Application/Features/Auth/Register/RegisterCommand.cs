using FlowDesk.Application.Common.Dto;
using MediatR;

namespace FlowDesk.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string UserName,
    string? AvatarUrl,
    string Password,
    string ConfirmPassword
) : IRequest<AuthResponseDto>;
