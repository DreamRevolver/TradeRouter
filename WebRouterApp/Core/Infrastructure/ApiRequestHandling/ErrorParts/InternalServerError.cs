using System;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts
{
    public sealed class InternalServerError : IApiError
    {
        public InternalServerError(Exception ex)
        {
            Title = ex.Message;
            Detail = ex.ToString();
        }
        
        public string Tag => ErrorTags.InternalServerError;
        public string Title { get; }
        public string Detail { get; }
    }
}