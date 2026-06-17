namespace FlowDesk.Application.Common.Dto;

public record UserSummaryDto(
    Guid UserId,
    string Username,
    string Email,
    string Role,
    string? AvatarUrl
);
