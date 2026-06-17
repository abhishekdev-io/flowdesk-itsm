using FlowDesk.Application.Behaviours;
using FlowDesk.Application.Common.Context;
using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Domain.Tickets.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FlowDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(SystemAdminBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PermissionBehaviour<,>));
        });
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly,
            includeInternalTypes: true);

        services.AddScoped<ICurrentWorkspaceContext,CurrentWorkspaceContext>();

        services.AddScoped<PermissionService>();

        services.AddScoped<SlaService>();    

        return services;
    }
}



