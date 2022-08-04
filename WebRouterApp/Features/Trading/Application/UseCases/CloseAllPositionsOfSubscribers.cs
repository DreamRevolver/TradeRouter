using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces;
using WebRouterApp.Core.Application.Services;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Shared.Types;
using ILogger = Shared.Interfaces.ILogger;

namespace WebRouterApp.Features.Trading.Application.UseCases
{
    public class CloseAllPositionsOfSubscribersApiCommand : IApiRequest<Unit>
    {
        public IReadOnlyList<Guid>? SubscriberIds { get; set; }
    }

    public class CloseAllPositionsOfSubscribersApiCommandHandler 
        : IApiRequestHandler<CloseAllPositionsOfSubscribersApiCommand, Unit>
    {
        private readonly ILogger _logger;
        private readonly ForEachSubscriber _forEachSubscriber;

        public CloseAllPositionsOfSubscribersApiCommandHandler(
            ForEachSubscriber forEachSubscriber, 
            ILogger logger)
        {
            _forEachSubscriber = forEachSubscriber;
            _logger = logger;
        }

        public Task<Unit> Handle(CloseAllPositionsOfSubscribersApiCommand command)
        {
            return _forEachSubscriber.Invoke(
                command.SubscriberIds, 
                async it =>
                {
                    if (!it.IsRunning)
                    {
                        _logger.Log(
                            LogPriority.Warning,
                            message: $"Cannot close all positions of " +
                                     $"the subscriber '{it.Record.Name}' ('{it.Id}'): it is not running.",
                            source: nameof(CloseAllPositionsOfSubscribersApiCommandHandler)
                        );
                        
                        return;
                    }

                    await it.CloseAllPositions();
                }
            );
        }
    }
}
