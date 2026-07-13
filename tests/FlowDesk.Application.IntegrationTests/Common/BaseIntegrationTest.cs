using FlowDesk.Application.IntegrationTests.Common.Helper;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FlowDesk.Application.IntegrationTests.Common;

//Instead of Create HttpClient,Create DbContext service, Reset Database and Seed user for every test repeateadly
// all the test classes can inherit from BaseIntegrationTest class to get these functionalities

[CollectionDefinition("IntegrationTests Collection")] //Here we create a collection named "IntegrationTests Collection"
public class SharedTestCollection : ICollectionFixture<IntegrationTestWebAppFactory> { } //This class says all the test class in the "IntegrationTests Collection" share same Fixture which is <IntegrationTestWebAppFactory>

[Collection("IntegrationTests Collection")]//Add the class which inherit BaseIntegrationTest belongs to "IntegrationTests Collection"
public abstract class BaseIntegrationTest: IAsyncLifetime
{
    protected readonly IntegrationTestWebAppFactory Factory;
    private readonly IServiceScope _scope;

    protected readonly HttpClient HttpClient;
    protected readonly ApplicationDbContext dbContext;
    protected readonly Guid TestUserId = TestUserDataSeeder.TestUserId;

    protected readonly TestDataBuilder TestSeeder;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    //Create the HttpClient,DbContext instance for each test classes
    {
        Factory = factory;
        HttpClient = factory.CreateClient();

        _scope = factory.Services.CreateScope();
        dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        TestSeeder = new TestDataBuilder(dbContext);
    }

    public async Task InitializeAsync() //Calls initiallize before test which Reset database and seed
    {
        await Factory.ResetDatabaseAsync();
        await Factory.Seeder.SeedTestUserAsync(); 
    }

    public Task DisposeAsync() //after test dispose the DbContext instance
    {
        _scope.Dispose();
        dbContext.Dispose();
        return Task.CompletedTask;
    }
}
 