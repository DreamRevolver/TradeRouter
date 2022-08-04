using System.Threading.Tasks;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts
{
    // ReSharper disable once UnusedTypeParameter
    public interface IApiRequest<TApiResponse>
    {
    }

    public interface IApiRequestHandler<in TApiRequest, TApiResponse>
        where TApiRequest : IApiRequest<TApiResponse>
    {
        Task<TApiResponse> Handle(TApiRequest request);
    }
}
