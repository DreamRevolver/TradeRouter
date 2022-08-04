using System;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts
{
    public sealed class NotFoundError : IApiError
    {
        public NotFoundError(Guid id, string message)
        {
            Id = id;
            Message = message;
        }
        
        public string Tag => ErrorTags.NotFoundError;
        public Guid Id { get; }
        public string Message { get; }
    }
}