namespace FlowDesk.Infrastructure.Identity.Config;

public class RefreshTokenCookieSettings
{
    public const string ConfigurationKey = "RefreshTokenCookie";
    
    public string Name { get; set; } = "refreshToken";
    public string Path { get; set; } = "/api/v1/auth";
    public string SameSite { get; set; } = "None";
}
