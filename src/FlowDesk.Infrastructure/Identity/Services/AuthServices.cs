using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using FlowDesk.Infrastructure.Identity.Entity;
using FlowDesk.Infrastructure.Identity.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace FlowDesk.Infrastructure.Identity.Services;

public class AuthServices(UserManager<AppUser> userManager, JwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = await jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.UserName!);

        return new AuthResponseDto(user.Id, user.UserName!, user.Email!, token);
    }

    public async Task<AuthResponseDto> RegisterAsync(string userName, string email, string password, string avatarUrl)
    {
        var user = AppUser.Create(userName, email, avatarUrl);
        var result = await userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)      
        {
            throw new IdentityException(result.Errors.Select(e => e.Description));
        }
        
        var token = await jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.UserName!);

        return new AuthResponseDto(user.Id, user.UserName!, user.Email!, token);
    }

    public async Task<Guid> RegisterSystemAdminAsync(string userName, string email, string password, string avatarUrl)
    {
        var user = AppUser.CreateSystemAdmin(userName, email, avatarUrl);
        var result = await userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)      
        {
            throw new IdentityException(result.Errors.Select(e => e.Description));
        }
        
        return user.Id;
    }
}


