using System.Collections.Generic;
using System.Threading.Tasks;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Publishers.Application.Models;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Publishers.Application.UseCases
{
    public sealed class PublisherRecordsApiQuery : IApiRequest<IReadOnlyList<PublisherRecord>>
    {
    }
    
    public class PublisherRecordListApiQueryHandler 
        : IApiRequestHandler<PublisherRecordsApiQuery, IReadOnlyList<PublisherRecord>>
    {
        private readonly TradeRouterDbContext _dbContext;

        public PublisherRecordListApiQueryHandler(TradeRouterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IReadOnlyList<PublisherRecord>> Handle(PublisherRecordsApiQuery query)
        {
            var subscriberRecords = _dbContext.Publishers.ToReadOnlyList();
            return Task.FromResult(subscriberRecords);
        }
    }
}