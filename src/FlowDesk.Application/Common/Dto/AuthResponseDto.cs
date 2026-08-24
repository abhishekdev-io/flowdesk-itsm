namespace FlowDesk.Application.Common.Dto;

public record AuthResponseDto(
    Guid UserId,
    string UserName,
    string Email,
    string AvatarUrl,
    string AccessToken,
    string RefreshToken
);


 