namespace FlowDesk.Application.Common.Dto;

public record AppUserDto(
    Guid UserId,
    string? Username,
    string? Email,
    string? AvatarUrl
);
