namespace FlowDesk.Api.Dto.SystemAdmin;

public sealed record RegisterSystemAdminRequest(
    string Email,
    string Username,
    string Password,
    string ConfirmPassword,
    string AvatarUrl);
