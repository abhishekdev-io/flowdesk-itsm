using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Authentication;
using FlowDesk.Infrastructure.Identity.Config;
using FlowDesk.Infrastructure.Identity.Entity;
using FlowDesk.Infrastructure.Identity.Exceptions;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FlowDesk.Infrastructure.Identity.Services;

public class AuthServices(
    UserManager<AppUser> userManager, 
    IJwtTokenGenerator jwtTokenGenerator, 
    ApplicationDbContext context,
    IOptions<JwtSettings> jwtOptions
    ) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    public async Task<AuthResponseDto> LoginAsync(
        string email, 
        string password,
        CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct)
    {
        var incomingHash = jwtTokenGenerator.HashToken(refreshToken);

        var existing = await context.RefreshTokens
        .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, ct)
         ?? throw new UnauthorizedAccessException("Invalid refresh token");
        
        if (existing.IsRevoked)
        {
            var activeTokens = await context.RefreshTokens
                .Where(t => t.UserId == existing.UserId && t.RevokedAt == null)
                .ToListAsync(ct);

            foreach (var t in activeTokens)
                t.Revoke("Reuse detected");

            await context.SaveChangesAsync(ct);
            throw new UnauthorizedAccessException("Token reuse detected. All sessions revoked.");
        }

        if (existing.IsExpired)
        throw new UnauthorizedAccessException("Refresh token expired");

        var user = await userManager.FindByIdAsync(existing.UserId.ToString())
        ?? throw new UnauthorizedAccessException("User not found");

        existing.Revoke("Rotated");
        await context.SaveChangesAsync(ct);

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponseDto> RegisterAsync(
        string userName, 
        string email, 
        string password, 
        string? avatarUrl,
        CancellationToken ct)
    {
            
        var user = AppUser.Create(userName, email, avatarUrl);
        var result = await userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)      
        {
            throw new IdentityException(result.Errors.Select(e => e.Description));
        }

        return await IssueTokensAsync(user, ct);
    }

    public async Task<Guid> RegisterSystemAdminAsync(
        string userName, 
        string email, 
        string password, 
        string? avatarUrl,
        CancellationToken ct)
    {
        
        var user = AppUser.CreateSystemAdmin(userName, email, avatarUrl);
        var result = await userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)      
        {
            throw new IdentityException(result.Errors.Select(e => e.Description));
        }
        
        return user.Id;
    }

    private async Task<AuthResponseDto> IssueTokensAsync(
        AppUser user,
        CancellationToken ct)
    {
        var accessToken = await jwtTokenGenerator.GenerateToken(
            user.Id, 
            user.Email!, 
            user.UserName!
            );

        var (rawToken, newHash) = jwtTokenGenerator.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(
            tokenHash: newHash,
            userId: user.Id,
            expiresAt: DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays));

        await context.RefreshTokens.AddAsync(refreshToken,ct);
        await context.SaveChangesAsync(ct);

        return new AuthResponseDto(
            user.Id, 
            user.UserName!, 
            user.Email!, 
            user.AvatarUrl!,
            accessToken, 
            rawToken
            );
    }
}
