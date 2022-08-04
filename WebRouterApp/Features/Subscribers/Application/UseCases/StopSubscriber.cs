using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces;
using WebRouterApp.Core.Application.Services;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Subscribers.Application.Services;
using WebRouterApp.Shared.Types;

namespace WebRouterApp.Features.Subscribers.Application.UseCases
{
    public sealed class StopSubscriberApiCommand : IApiRequest<Unit>
    {
        public IReadOnlyList<Guid>? SubscriberIds { get; set; }
    }

    public class StopSubscriberApiCommandHandler : IApiRequestHandler<StopSubscriberApiCommand, Unit>
    {
        private readonly ForEachSubscriber _forEachSubscriber;
        private readonly ILogger _log;
        private readonly TraderLock _traderLock;

        public StopSubscriberApiCommandHandler(
            ForEachSubscriber forEachSubscriber,
            ILogger log,
            TraderLock traderLock)
        {
            _forEachSubscriber = forEachSubscriber;
            _log = log;
            _traderLock = traderLock;
        }

        public Task<Unit> Handle(StopSubscriberApiCommand command)
        {
            return _forEachSubscriber.Invoke(
                command.SubscriberIds,
                async it =>
                {
                    // We must lock the subscribers, so they are not going to
                    // get updated/deleted while we're stopping them.
                    using (await _traderLock.For(it.Id).LockAsync())
                    {
                        if (!it.IsRunning)
                        {
                            _log.Log(
                                LogPriority.Info,
                                source: nameof(StartSubscriberApiCommandHandler),
                                message: $"'{it.Record.Name}' ('{it.Record.Id}') has not been started.");
                            return;
                        }

                        await it.Stop();
                    }
                });
        }
    }
}
