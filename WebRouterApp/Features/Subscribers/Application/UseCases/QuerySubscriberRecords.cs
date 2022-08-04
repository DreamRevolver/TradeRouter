using System.Collections.Generic;
using System.Threading.Tasks;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Subscribers.Application.Models;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Subscribers.Application.UseCases
{
    public sealed class SubscriberRecordsApiQuery : IApiRequest<IReadOnlyList<SubscriberRecord>>
    {
    }
    
    public class SubscriberRecordListApiQueryHandler 
        : IApiRequestHandler<SubscriberRecordsApiQuery, IReadOnlyList<SubscriberRecord>>
    {
        private readonly TradeRouterDbContext _dbContext;

        public SubscriberRecordListApiQueryHandler(TradeRouterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IReadOnlyList<SubscriberRecord>> Handle(SubscriberRecordsApiQuery query)
        {
            var subscriberRecords = _dbContext.Subscribers.ToReadOnlyList();
            return Task.FromResult(subscriberRecords);
        }
    }
}