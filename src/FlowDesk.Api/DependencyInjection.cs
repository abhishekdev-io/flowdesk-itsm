using FlowDesk.Api.Realtime;
using FlowDesk.Application;
using FlowDesk.Application.Interfaces.Notifications;
using FlowDesk.Infrastructure;

namespace FlowDesk.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
        {
            services.AddInfrastructure(config).AddApplication();
            services.AddScoped<IRealtimeNotificationPublisher, RealtimeNotificationPublisher>();
            return services;
        }
    }
}