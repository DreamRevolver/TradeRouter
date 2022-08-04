using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Trading.Application.UseCases;

namespace WebRouterApp.Features.Trading.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class TradingController : Controller
    {
        private readonly EndpointHandler<TradingController> _endpointHandler;

        public TradingController(EndpointHandler<TradingController> endpointHandler)
        {
            _endpointHandler = endpointHandler;
        }

        [HttpPost]
        public Task<IActionResult> RequestTradingSnapshot([FromBody] RequestTradingSnapshotApiCommand command) => 
            _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> CancelAllOrdersOfSubscribers([FromBody] CancelAllOrdersOfSubscribersApiCommand command) => 
            _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> CloseAllPositionsOfSubscribers([FromBody] CloseAllPositionsOfSubscribersApiCommand command) => 
            _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> SyncPositionsOfSubscribers([FromBody] SyncPositionsOfSubscribersApiCommand command) => 
            _endpointHandler.Handle(command);

        [HttpPost]
        public Task<IActionResult> SyncOrdersOfSubscribers([FromBody] SyncOrdersOfSubscribersApiCommand command) => 
            _endpointHandler.Handle(command);
    }
}
