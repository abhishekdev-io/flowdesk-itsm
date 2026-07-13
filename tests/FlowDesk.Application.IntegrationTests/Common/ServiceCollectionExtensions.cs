using Microsoft.Extensions.DependencyInjection;

namespace FlowDesk.Application.IntegrationTests.Common;

public static class ServiceCollectionExtensions
{
    //Overload of RemoveByServiceType method that takes a Type parameter instead of a generic type parameter, allowing us to remove services by their runtime type.
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
