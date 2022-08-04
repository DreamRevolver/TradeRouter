using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts
{
    public static class EndpointHandlerServiceCollectionExtensions
    {
        public static void AddEndpointHandler(this IServiceCollection services)
        {
            services.AddSingleton(typeof(EndpointHandler<>));
            services.AddSingleton(typeof(ApiRequestDispatcher));
            services.AddSingleton(typeof(ApiRequestHandlerThunk<,>));
        }

        public static void AddApiRequestHandlersFromAssemblyContaining<TType>(
            this IServiceCollection services)
        {
            var handlers = typeof(TType).Assembly
                .GetTypes()
                .SelectMany(type => type.GetInterfaces()
                    .Where(@interface => 
                        @interface.IsGenericType &&
                        @interface.GetGenericTypeDefinition() == typeof(IApiRequestHandler<,>)
                    )
                    .Select(@interface => (service: @interface, implementation: type))
                )
                .ToReadOnlyList();

            foreach (var (service, implementation) in handlers)
            {
                services.AddTransient(service, implementation);
            }
        }
    }
}