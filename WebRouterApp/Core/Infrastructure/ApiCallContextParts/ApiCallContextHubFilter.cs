using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace WebRouterApp.Core.Infrastructure.ApiCallContextParts
{
    public class ApiCallContextHubFilter : IHubFilter
    {
        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            return next(context);
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext, 
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            // Console.WriteLine($"Calling hub method '{invocationContext.HubMethodName}'");
            try
            {
                var claimsPrincipal = invocationContext.Context.User;
                if (claimsPrincipal != null)
                {
                    invocationContext.ServiceProvider
                        .GetRequiredService<ApiCallContext>()
                        .InitializeWith(claimsPrincipal);
                }
                
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception calling '{invocationContext.HubMethodName}': {ex}");
                throw;
            }
        }

        public Task OnDisconnectedAsync(
            HubLifetimeContext context, 
            Exception? exception, 
            Func<HubLifetimeContext, Exception?, Task> next)
        {
            return next(context, exception);
        }
    }
}