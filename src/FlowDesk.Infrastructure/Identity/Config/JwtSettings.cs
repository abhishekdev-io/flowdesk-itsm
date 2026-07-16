namespace FlowDesk.Infrastructure.Identity.Config;

public class JwtSettings
{
    public const string ConfigurationKey = "JWT";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; }
    public int RefreshTokenExpiryInDays { get; set; }
}
