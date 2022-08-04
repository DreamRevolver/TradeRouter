using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces;
using WebRouterApp.Core.Application.Services;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Trading.Application.UseCases
{
    public class SyncOrdersOfSubscribersApiCommand : IApiRequest<Unit>
    {
        public IReadOnlyList<Guid>? SubscriberIds { get; set; }
    }
    
    public class SyncOrdersOfSubscribersApiCommandHandler 
        : IApiRequestHandler<SyncOrdersOfSubscribersApiCommand, Unit>
    {
        private readonly ILogger _logger;
        private readonly ForEachSubscriber _forEachSubscriber;

        public SyncOrdersOfSubscribersApiCommandHandler(
            ForEachSubscriber forEachSubscriber, 
            ILogger logger)
        {
            _forEachSubscriber = forEachSubscriber;
            _logger = logger;
        }

        public Task<Unit> Handle(SyncOrdersOfSubscribersApiCommand command)
        {
            return _forEachSubscriber.Invoke(
                command.SubscriberIds, 
                async it =>
                {
                    if (!it.IsRunning)
                    {
                        _logger.Log(
                            LogPriority.Warning,
                            message: $"Cannot sync orders of " +
                                     $"the subscriber '{it.Record.Name}' ('{it.Id}'): it is not running.",
                            source: nameof(SyncOrdersOfSubscribersApiCommandHandler)
                        );
                        
                        return;
                    }

                    await it.SynchronizeOrders();
                }
            );
        }
    }
}
