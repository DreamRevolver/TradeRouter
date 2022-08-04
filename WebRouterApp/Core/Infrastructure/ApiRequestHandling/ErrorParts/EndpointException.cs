using System;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts
{
    public class EndpointException : Exception
    {
        public EndpointException(IApiError error)
        {
            Error = error;
        }

        public IApiError Error { get; }
    }
}