using FlowDesk.Application.Interfaces;
using FlowDesk.Infrastructure.Identity.Services;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FlowDesk.Infrastructure.Identity.Extensions;
using FlowDesk.Infrastructure.Persistence.Repository;
using FlowDesk.Domain.Workspaces;
using FlowDesk.Infrastructure.Identity.Context;
using FlowDesk.Infrastructure.Identity.Repository;
using FlowDesk.Domain.Tickets;
using FlowDesk.Infrastructure.Identity.Seeders;
using FlowDesk.Infrastructure.Persistence.Seeders;
using FlowDesk.Infrastructure.Persistence.Services;
using FlowDesk.Infrastructure.Identity.Config;
using FlowDesk.Application.Interfaces.Notifications;
using FlowDesk.Application.Interfaces.Realtime;

namespace FlowDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
    IConfiguration config)
    {
        services.AddDbContextServices(config);

        services.AddDataProtection();

        services.AddIdentityServices();

        services.AddAuthServices(config);

        services.AddAuthorization();  

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>()
                .AddScoped<IAuthService, AuthServices>()
                .AddScoped<ICurrentUserContext, CurrentUserContext>()
                .AddScoped<IWorkspaceRepository, WorkspaceRepository>()
                .AddScoped<IAppUserRepository, AppUserRepository>()
                .AddScoped<ITicketRepository,TicketRepository>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<ITicketActivityService, TicketActivityService>()
                .AddScoped<IMembershipChecker, MembershipChecker>();

        services.AddScoped<SystemAdminSeeder>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeederService>();

        return services;
    }
}


