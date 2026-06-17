namespace FlowDesk.Api.Dto.Auth;

public sealed record LoginRequest(      
    string Email,
    string Password);
