using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
 
namespace FlowDesk.Infrastructure.Persistence.Seeders;
    
public static class ItsmUserSeeder
{
    public static async Task<List<AppUser>> SeedPlatformUsersAsync(UserManager<AppUser> userManager)
    {
        var users = await userManager.Users.Where(u => u.Email != "superadmin@flowdesk.com").ToListAsync();
        if (users.Count != 0) return users;

        var localAdmin = AppUser.Create("workspaceadmin@itsm.com","workspaceadmin@itsm.com","/images/workspaceadmin.jpg");

        var agent1 = AppUser.Create("alex@itsm.com","alex@itsm.com","/images/Alex.jpg");
        var agent2 = AppUser.Create("tylor@itsm.com","tylor@itsm.com","/images/Tylor.jpg");

        var member1 = AppUser.Create("linda@itsm.com","linda@itsm.com","/images/Linda.jpg");
        var member2 = AppUser.Create("ericlee@itsm.com","ericlee@itsm.com","/images/EricLee.jpg");
        var member3 = AppUser.Create("williams@itsm.com","williams@itsm.com","/images/Williams.jpg");

        await userManager.CreateAsync(localAdmin, "LocalAdminPass123!");
        await userManager.CreateAsync(agent1, "AlexPass123!");
        await userManager.CreateAsync(agent2, "TylorPass123!");
        await userManager.CreateAsync(member1, "LindaPass123!");
        await userManager.CreateAsync(member2, "EricPass123!");
        await userManager.CreateAsync(member3, "WilliamsPass123!");

        return [localAdmin, agent1, agent2, member1, member2, member3];
    }
}    
   
             