using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces;
using WebRouterApp.Core.Application.Models;
using WebRouterApp.Core.Application.Services;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Subscribers.Application.Services;

namespace WebRouterApp.Features.Subscribers.Application.UseCases
{
    public sealed class StartSubscriberApiCommand : IApiRequest<StartSubscriberApiResponse>
    {
        public IReadOnlyList<Guid>? SubscriberIds { get; set; }
    }

    public sealed class StartSubscriberApiResponse
    {
        public StartSubscriberApiResponse(IReadOnlyList<SubscriberStatusOverview> statusOverviews)
        {
            StatusOverviews = statusOverviews;
        }

        public IReadOnlyList<SubscriberStatusOverview> StatusOverviews { get; }
    }

    public sealed class SubscriberStatusOverview
    {
        public SubscriberStatusOverview(Guid id, TraderStatus status)
        {
            Id = id;
            Status = status;
        }

        public Guid Id { get; }
        public TraderStatus Status { get; }
    }

    public class StartSubscriberApiCommandHandler 
        : IApiRequestHandler<StartSubscriberApiCommand, StartSubscriberApiResponse>
    {
        private readonly ForEachSubscriber _forEachSubscriber;
        private readonly ILogger _log;
        private readonly TraderLock _traderLock;

        public StartSubscriberApiCommandHandler(
            ForEachSubscriber forEachSubscriber,
            ILogger log,
            TraderLock traderLock)
        {
            _forEachSubscriber = forEachSubscriber;
            _log = log;
            _traderLock = traderLock;
        }

        public async Task<StartSubscriberApiResponse> Handle(StartSubscriberApiCommand command)
        {
            await _forEachSubscriber.Invoke(
                command.SubscriberIds,
                async it =>
                {
                    // We must lock the subscribers, so they are not going to
                    // get updated/deleted while we're starting them.
                    using (await _traderLock.For(it.Id).LockAsync())
                    {
                        if (it.IsRunning)
                        {
                            _log.Log(
                                LogPriority.Info,
                                source: nameof(StartSubscriberApiCommandHandler),
                                message: $"'{it.Record.Name}' ('{it.Record.Id}') has already been started.");
                            return;
                        }

                        await it.Start();
                    }
                });

            // Sometimes a subscriber goes into the running state later than awaiting `it.Start()` finishes.
            // What do you do? Let's await on `Task.Delay` to accomodate cases like this.
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            var statusOverviews = new List<SubscriberStatusOverview>();
            await _forEachSubscriber.Invoke(
                command.SubscriberIds,
                it =>
                {
                    statusOverviews.Add(new SubscriberStatusOverview(
                        it.Id,
                        it.IsRunning ? TraderStatus.Running : TraderStatus.Stopped));
                    return Task.CompletedTask;
                });

            return new StartSubscriberApiResponse(statusOverviews);
        }
    }
}
