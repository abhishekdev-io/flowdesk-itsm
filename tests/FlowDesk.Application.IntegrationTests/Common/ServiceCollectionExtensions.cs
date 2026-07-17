using Microsoft.Extensions.DependencyInjection;

namespace FlowDesk.Application.IntegrationTests.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RemoveByServiceType<TService>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(TService));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        return services;
    }
}
