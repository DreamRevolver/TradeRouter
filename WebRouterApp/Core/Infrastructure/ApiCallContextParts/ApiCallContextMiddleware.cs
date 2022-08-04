using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebRouterApp.Core.Infrastructure.ApiCallContextParts
{
    public class ApiCallContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiCallContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            httpContext.RequestServices
                .GetRequiredService<ApiCallContext>()
                .InitializeWith(httpContext.User);
            
            return _next(httpContext);
        }
    }

    public static class ApiCallContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiCallContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiCallContextMiddleware>();
        }
    }
}