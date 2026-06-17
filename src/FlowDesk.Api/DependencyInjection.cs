using FlowDesk.Application;
using FlowDesk.Infrastructure;

namespace FlowDesk.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
        {
            services.AddInfrastructure(config).AddApplication();
            return services;
        }
    }
}