using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using WebRouterApp.Core.Application.Models;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Publishers.Application.Models;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Publishers.Application.UseCases
{
    public sealed class PublisherOverviewsApiQuery : IApiRequest<IReadOnlyList<PublisherOverview>>
    {
    }

    public class PublisherOverviewsApiQueryHandler
        : IApiRequestHandler<PublisherOverviewsApiQuery, IReadOnlyList<PublisherOverview>>
    {
        private readonly IApiCallContextReader _apiCallContext;

        public PublisherOverviewsApiQueryHandler(IApiCallContextReader apiCallContext)
        {
            _apiCallContext = apiCallContext;
        }

        public Task<IReadOnlyList<PublisherOverview>> Handle(PublisherOverviewsApiQuery query)
        {
            var copySession = _apiCallContext.Session;
            if (copySession == null)
                throw new InvalidOperationException($"{nameof(copySession)} is null.");

            var publisher = copySession.Publisher;
            var publisherOverview = new PublisherOverview
            {
                Id = publisher.Id,
                Name = publisher.Record.Name,
                UtcStartedAt = publisher.UtcStartedAt,
                Ping = 0,
                FormattedBalance = publisher.FormatBalance(WalletCurrency.USDT),
                Description = publisher.Record.Description,
                // FrontEndClient.Start is void, so we can't await it.
                // We should really fix the core issue, but until then, we use the hack below.
                // As at the moment, we only have one publisher and it always gets started 
                // on login or SignalR connecting, we always report it as running.
                // Obviously, in case there are any errors while starting the publisher,
                // we'll be incorrectly reporting its status -- as running.
                Status = TraderStatus.Running
            };

            return Task.FromResult(new[] { publisherOverview }.ToReadOnlyList());
        }
    }
}
