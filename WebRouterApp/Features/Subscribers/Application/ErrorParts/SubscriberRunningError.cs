using System;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;

namespace WebRouterApp.Features.Subscribers.Application.ErrorParts
{
    public class SubscriberRunningError : IApiError
    {
        public SubscriberRunningError(Guid subscriberId, string message)
        {
            Message = message;
            SubscriberId = subscriberId;
        }

        public string Tag => ErrorTags.SubscriberRunningError;
        public Guid SubscriberId { get; }
        public string Message { get; }
    }
}
