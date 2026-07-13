using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FlowDesk.Application.IntegrationTests.Common.Helper;

//Seed User registration
public class TestUserDataSeeder(IServiceProvider serviceProvider)
{
    public static readonly Guid TestUserId = Guid.Parse("77777777-7777-7777-7777-777777777777");
    public const string TestUserEmail = "testuser@itsm.com";

    public async Task SeedTestUserAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        if (await userManager.FindByIdAsync(TestUserId.ToString()) is not null)
            return;
        
        var user = AppUser.Create(TestUserEmail, TestUserEmail);
        user.Id = TestUserId;

        var result = await userManager.CreateAsync(user, "TestPass123!");
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Test user seeding failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task<Guid> CreateAdditionalUserAsync(string email)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var user = AppUser.Create(email, email);
        var result = await userManager.CreateAsync(user, "TestPass456!");
        
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Additional user creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user.Id;
    }
}
