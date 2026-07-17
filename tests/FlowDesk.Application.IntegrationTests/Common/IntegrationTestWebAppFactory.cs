using System.Data.Common;
using FlowDesk.Application.IntegrationTests.Common.Helper;
using FlowDesk.Application.Interfaces;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;

namespace FlowDesk.Application.IntegrationTests.Common;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestConnectionString = "Host=localhost;Database=flowdesk_test;Username=postgres;Password=postgres;Include Error Detail=true";
    private DbConnection _dbConnection = default!;

    //Respawner clean database after each test
    private Respawner _respawner = default!;
    public TestUserDataSeeder Seeder { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        if (scope.ServiceProvider.GetRequiredService<IApplicationDbContext>() is DbContext dbContext)
            await dbContext.Database.MigrateAsync(); //makes dotnet ef database update automatic

        _dbConnection = new NpgsqlConnection(TestConnectionString);
        await _dbConnection.OpenAsync(); 

        //create respawner   
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });

        Seeder = new TestUserDataSeeder(Services); //Create a new instance of TestDataSeeder with the service provider
    }

    //Replaces the default ConfigureWebHost method to configure the test server and services for integration testing
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); //Tells application we are in testing environment

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT:Issuer"] = "FlowDesk.IntegrationTests",
                ["JWT:Audience"] = "FlowDesk.IntegrationTests",
                ["JWT:SecretKey"] = "Th1s-Is-A-Test-Only-Secret-Key-Min-32-Chars-Long!"
            });
        });
        
        //Here we configure the test services, including replacing the real database context with a test database context,
        // and adding a fake current user context for testing purposes.
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null) services.Remove(descriptor); 

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(TestConnectionString)); //Register test database
            
            services.RemoveByServiceType<IApplicationDbContext>(); 
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>()); 
            
            //Register a test authentication scheme to bypass real authentication during testing,
            // here we use TestAuthHandler instead of Jwt so every request is authenticated automatically
            services.AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName, options => { });
        });
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_dbConnection); //Used to call the Reset function of Respawner

    public new async Task DisposeAsync()
    {
        if (_dbConnection != null)
            await _dbConnection.DisposeAsync();
    }
}


