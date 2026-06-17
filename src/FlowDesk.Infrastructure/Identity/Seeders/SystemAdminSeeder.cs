using FlowDesk.Infrastructure.Identity.Entity;
using FlowDesk.Infrastructure.Identity.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FlowDesk.Infrastructure.Identity.Seeders;

public class SystemAdminSeeder(
    UserManager<AppUser> userManager,
    IConfiguration config)
{
    public  async Task SeedSystemAdminAsync()
    {
        var email = config["SystemAdmin:Email"];
        var username = config["SystemAdmin:Username"];
        var password = config["SystemAdmin:Password"];

        const string avatarUrl = "/images/systemadmin.png";

        if (await userManager.FindByEmailAsync(email!) is not null)
            return;

        var admin = AppUser.CreateSystemAdmin(email!, username!,avatarUrl);

        var result = await userManager.CreateAsync(admin, password!);

        if (!result.Succeeded)
        {
            throw new IdentityException(result.Errors.Select(e => e.Description));
        }
     
    }
}
