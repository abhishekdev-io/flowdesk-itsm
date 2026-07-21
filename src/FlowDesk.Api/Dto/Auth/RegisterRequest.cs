namespace FlowDesk.Api.Dto.Auth;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string ConfirmPassword,
    string AvatarUrl);
 