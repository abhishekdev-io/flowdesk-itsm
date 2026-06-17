using FlowDesk.Application.Interface;
using FlowDesk.Infrastructure.Identity.Entity;
using FlowDesk.Infrastructure.Identity.Seeders;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FlowDesk.Infrastructure.Persistence.Seeders;

public class DatabaseSeederService(
    ApplicationDbContext context, 
    UserManager<AppUser> userManager,
    SystemAdminSeeder systemAdminSeeder,
    ILogger<DatabaseSeederService> logger) 
    : IDatabaseSeeder
{    

    public async Task SeedAsync()
    {
        logger.LogInformation("👀Checking database status...");
        await context.Database.EnsureCreatedAsync();

        //seed systemadmin
        logger.LogInformation("Seeding System Administrator...");
        await systemAdminSeeder.SeedSystemAdminAsync();

        if (await context.Tickets.AnyAsync() || 
            await context.Workspaces.AnyAsync(w => w.NormalizedName == "ACME TECHNOLOGIES"))
        {
            logger.LogInformation("Database already contains data. Skipping demo data seeding.");
            return;
        }

        // Platform Identity User Base
        logger.LogInformation("Seeding Platform Identity Users...");
        var platformUsers = await ItsmUserSeeder.SeedPlatformUsersAsync(userManager);

        var localAdmin = platformUsers.First(u => u.Email == "workspaceadmin@itsm.com");
        var agent1 = platformUsers.First(u => u.Email == "alex@itsm.com");
        var agent2 = platformUsers.First(u => u.Email == "tylor@itsm.com");
        var member1 = platformUsers.First(u => u.Email == "linda@itsm.com");
        var member2 = platformUsers.First(u => u.Email == "ericlee@itsm.com");
        var member3 = platformUsers.First(u => u.Email == "williams@itsm.com");

        logger.LogInformation("Generating Workspace and Assets...");
        var (workspace, seededAssets) = await WorkspaceSeeder.SeedWorkspaceContextAsync(
            context, 
            localAdmin, 
            agent1, 
            agent2, 
            member1, 
            member2, 
            member3);

        logger.LogInformation("Populating Ticket System...");
        await TicketSystemSeeder.SeedTicketAsync(
            context, 
            workspace, 
            seededAssets, 
            localAdmin,
            agent1, 
            agent2, 
            member1, 
            member2, 
            member3);    

        logger.LogInformation("✅ Database seeding completed successfully.");    

    }
}
