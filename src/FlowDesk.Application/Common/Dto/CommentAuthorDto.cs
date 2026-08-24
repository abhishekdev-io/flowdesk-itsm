namespace FlowDesk.Application.Common.Dto;

public record CommentAuthorDto(
    Guid UserId,
    string Username,
    string Role,
    string? AvatarUrl
);
