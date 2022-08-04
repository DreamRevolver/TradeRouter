using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebRouterApp.Shared;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts
{
    public sealed class ApiRequestDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public ApiRequestDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static readonly ConcurrentDictionary<Type, object> RequestToHandlerThunkMap = new();
        public async Task<TApiResponse> Send<TApiResponse>(IApiRequest<TApiResponse> request)
        {
            var handler = GetOrCreateHandlerThunk<TApiResponse>(request.GetType());
            var response = await handler.Handle(request);
            return response;
        }

        private IApiRequestHandlerThunk<TApiResponse> GetOrCreateHandlerThunk<TApiResponse>(Type requestType)
        {
            var thunk = (IApiRequestHandlerThunk<TApiResponse>) RequestToHandlerThunkMap.GetOrAdd(
                requestType, 
                _ => CreateHandlerThunk<TApiResponse>(requestType));
            return thunk;
        }

        private IApiRequestHandlerThunk<TApiResponse> CreateHandlerThunk<TApiResponse>(Type requestType)
        {
            var thunkType = typeof(ApiRequestHandlerThunk<,>).MakeGenericType(requestType, typeof(TApiResponse));
            var thunk = (IApiRequestHandlerThunk<TApiResponse>)_serviceProvider.GetRequiredService(thunkType);
            return thunk;
        }
    }
    
    public interface IApiRequestHandlerThunk<TApiResponse>
    {
        Task<TApiResponse> Handle(IApiRequest<TApiResponse> request);
    }

    public sealed class ApiRequestHandlerThunk<TApiRequest, TApiResponse> : IApiRequestHandlerThunk<TApiResponse>
        where TApiRequest : IApiRequest<TApiResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ApiRequestHandlerThunk(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<TApiResponse> Handle(IApiRequest<TApiResponse> request)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            CodeAsserts.NotNull(httpContext);

            var handler = httpContext
                .RequestServices
                .GetRequiredService<IApiRequestHandler<TApiRequest, TApiResponse>>();
            
            return handler.Handle((TApiRequest)request);
        }
    }
}
