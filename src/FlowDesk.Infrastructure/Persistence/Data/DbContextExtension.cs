using FlowDesk.Infrastructure.Persistence.Data.Interceptor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowDesk.Infrastructure.Persistence.Data;

public static class DbContextExtension
{
    public static void AddDbContextServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<PublishDomainEventsInterceptor>();
        
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .AddInterceptors(serviceProvider.GetRequiredService<PublishDomainEventsInterceptor>()); 
        });
    }
}
