using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;

namespace WebRouterApp.Core.Infrastructure
{
    public class AppExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AppExceptionHandlerMiddleware> _log;

        public AppExceptionHandlerMiddleware(RequestDelegate next, ILogger<AppExceptionHandlerMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _log.LogError(exception: ex, message: "An internal server error encountered");

                var internalServerError = new InternalServerError(ex);

                var response = context.Response;

                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await response.WriteAsync(JsonSerializer.Serialize(internalServerError));
            }
        }
    }

    public static class AppExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder app) 
            => app.UseMiddleware<AppExceptionHandlerMiddleware>();
    }
}