using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FlowDesk.Infrastructure.Identity.Services;

public class JwtTokenGenerator
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;
    public JwtTokenGenerator(IConfiguration config, UserManager<AppUser> userManager)
    {
        _config = config;
        _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
        _userManager = userManager;
    }

    public async Task<string> GenerateToken(Guid userId, string email, string userName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("User not found");

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString()),
            new (ClaimTypes.Email, email),
            new (ClaimTypes.Name, userName)
        };

        if (user.IsSystemAdmin)
            claims.Add(new Claim("is_system_admin", "true"));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

 