using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Subscribers.Application.UseCases;
using WebRouterApp.Features.Subscribers.Application.UseCases.QuerySubscriberOverviews;

namespace WebRouterApp.Features.Subscribers.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SubscribersController : Controller
    {
        private readonly EndpointHandler<SubscribersController> _endpointHandler;
        
        public SubscribersController(EndpointHandler<SubscribersController> endpointHandler)
        {
            _endpointHandler = endpointHandler;
        }

        [HttpPost]
        public Task<IActionResult> GetOverviews([FromBody] SubscriberOverviewsApiQuery query) => _endpointHandler.Handle(query);

        [HttpPost]
        public Task<IActionResult> GetRecords([FromBody] SubscriberRecordsApiQuery query) => _endpointHandler.Handle(query);

        [HttpPost]
        public Task<IActionResult> Create([FromBody] CreateSubscriberApiCommand command) => _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> Update([FromBody] UpdateSubscriberApiCommand command) => _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> Delete([FromBody] DeleteSubscriberApiCommand command) => _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> Start([FromBody] StartSubscriberApiCommand command) => _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> Stop([FromBody] StopSubscriberApiCommand command) => _endpointHandler.Handle(command);
    }
}
