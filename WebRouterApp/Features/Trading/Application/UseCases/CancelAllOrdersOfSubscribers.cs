using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces;
using WebRouterApp.Core.Application.Services;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Trading.Application.UseCases
{
    public class CancelAllOrdersOfSubscribersApiCommand : IApiRequest<Unit>
    {
        public IReadOnlyList<Guid>? SubscriberIds { get; set; }
    }

    public class CancelAllOrdersOfSubscribersApiCommandHandler
        : IApiRequestHandler<CancelAllOrdersOfSubscribersApiCommand, Unit>
    {
        private readonly ForEachSubscriber _forEachSubscriber;
        private readonly ILogger _logger;

        public CancelAllOrdersOfSubscribersApiCommandHandler(
            ForEachSubscriber forEachSubscriber,
            ILogger logger)
        {
            _forEachSubscriber = forEachSubscriber;
            _logger = logger;
        }

        public Task<Unit> Handle(CancelAllOrdersOfSubscribersApiCommand command)
        {
            return _forEachSubscriber.Invoke(
                command.SubscriberIds,
                async it =>
                {
                    if (!it.IsRunning)
                    {
                        _logger.Log(
                            LogPriority.Warning,
                            "Cannot cancel all orders of " +
                            $"the subscriber '{it.Record.Name}' ('{it.Id}'): it is not running.",
                            nameof(CancelAllOrdersOfSubscribersApiCommandHandler)
                        );
                        return;
                    }

                    await it.CancelAllOrders();
                }
            );
        }
    }
}
