namespace FlowDesk.Infrastructure.Identity.Config;

public class RefreshTokenCookieSettings
{
    public const string ConfigurationKey = "JWT";
    
    public string Name { get; set; } = "refreshToken";
    public string Path { get; set; } = "/api/auth";
    public string SameSite { get; set; } = "None";
}
