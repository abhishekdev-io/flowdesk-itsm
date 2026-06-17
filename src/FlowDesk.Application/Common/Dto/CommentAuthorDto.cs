namespace FlowDesk.Application.Common.Dto;

public record CommentAuthorDto(
    string Username,
    string Role,
    string? AvatarUrl
);
