namespace FlowDesk.Application.Common.Dto;

public record AuthResponseDto(
    Guid UserId,
    string UserName,
    string Email,
    string Token
);


