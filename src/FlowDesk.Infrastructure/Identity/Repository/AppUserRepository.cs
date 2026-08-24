using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Infrastructure.Identity.Repository;

public class AppUserRepository(
    UserManager<AppUser> userManager,
    ICurrentUserContext currentUserContext) : IAppUserRepository
{
    private readonly UserManager<AppUser> _userManager = userManager;

    public async Task<AppUserDto?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = currentUserContext.UserId;

        return await _userManager.Users
               .AsNoTracking()
               .Where(u => u.Id == userId)
               .Select(u => new AppUserDto(
                   u.Id,
                   u.UserName,
                   u.Email,
                   u.AvatarUrl
               )).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AppUserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _userManager.Users
               .AsNoTracking()
               .Where(u => u.Id == userId)
               .Select(u => new AppUserDto(
                   u.Id,
                   u.UserName,
                   u.Email,
                   u.AvatarUrl
               )).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<AppUserDto>> ListUsersAsync(CancellationToken cancellationToken)
    {
        return await _userManager.Users
                .AsNoTracking()
                .Where(u => !u.IsSystemAdmin)
                .Select(u => new AppUserDto(
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.AvatarUrl
                )).ToListAsync(cancellationToken);
    }
}
