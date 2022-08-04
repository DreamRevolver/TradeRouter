using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Models;
using WebRouterApp.Core.Application.Models;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Subscribers.Application.UseCases.QuerySubscriberOverviews
{
    public sealed class SubscriberOverviewsApiQuery : IApiRequest<IReadOnlyList<SubscriberOverview>>
    {
        public IReadOnlyList<Guid>? SubscriberIds { get; set; }
    }

    public class SubscriberOverviewsApiQueryHandler
        : IApiRequestHandler<SubscriberOverviewsApiQuery, IReadOnlyList<SubscriberOverview>>
    {
        private readonly IApiCallContextReader _apiCallContext;

        public SubscriberOverviewsApiQueryHandler(IApiCallContextReader apiCallContext)
        {
            _apiCallContext = apiCallContext;
        }

        public Task<IReadOnlyList<SubscriberOverview>> Handle(SubscriberOverviewsApiQuery query)
        {
            var copySession = _apiCallContext.Session;
            if (copySession == null)
                throw new InvalidOperationException($"{nameof(copySession)} is null.");

            var subscriberIds = query.SubscriberIds ??
                                copySession.Publisher.Subscribers.Select(it => it.Id).ToReadOnlyList();

            var overviews = copySession.Publisher.Subscribers
                .Where(it => subscriberIds.Contains(it.Id))
                .Select(it => new SubscriberOverview
                {
                    Id = it.Record.Id,
                    Name = it.Record.Name,
                    UtcStartedAt = it.UtcStartedAt,
                    Ping = 0,
                    FormattedBalance = it.FormatBalance(WalletCurrency.USDT),
                    Description = "...",
                    Multiplier = it.Record.Multiplier,
                    Status = it.IsRunning ? TraderStatus.Running : TraderStatus.Stopped
                })
                .ToReadOnlyList();

            return Task.FromResult(overviews);
        }
    }
}
