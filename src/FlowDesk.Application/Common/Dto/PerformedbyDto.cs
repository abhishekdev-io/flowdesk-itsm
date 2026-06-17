namespace FlowDesk.Application.Common.Dto;

public record PerformedbyDto(
    string Username,
    string Role,
    string? AvatarUrl
);
