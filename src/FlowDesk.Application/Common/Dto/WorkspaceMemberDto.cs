using FlowDesk.Domain.Workspaces.Enums;

namespace FlowDesk.Application.Common.Dto;

public record WorkspaceMemberDto(
    Guid UserId,
    string? Username,
    string? Email,
    string? AvatarUrl,
    string Role
);
