using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebRouterApp.Core.CopyEngineParts;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Shared.Collections;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Core.Application.Services
{
    public class ForEachSubscriber
    {
        private readonly ILogger<ForEachSubscriber> _logger;
        private readonly IApiCallContextReader _apiCallContext;

        public ForEachSubscriber(
            ILogger<ForEachSubscriber> logger, 
            IApiCallContextReader apiCallContext)
        {
            _logger = logger;
            _apiCallContext = apiCallContext;
        }

        public async Task<Unit> Invoke(IReadOnlyList<Guid>? subscriberIds, Func<Subscriber, Task> func)
        {
            var copySession = _apiCallContext.Session;
            if (copySession == null)
                return Unit.Value;
            
            // `subscriberIds == null` or `subscriberIds == []` means all subscribers. 
            if (subscriberIds == null || subscriberIds.IsEmpty())
            {
                subscriberIds = copySession.Publisher.Subscribers
                    .Select(it => it.Record.Id)
                    .ToList()
                    .AsReadOnly();
            }

            var idToSubscriberMap = copySession.Publisher.Subscribers.ToDictionary(it => it.Record.Id);

            foreach (var id in subscriberIds)
            {
                var subscriber = idToSubscriberMap[id];
                await func(subscriber);
            }

            return Unit.Value;
        }
    }
}