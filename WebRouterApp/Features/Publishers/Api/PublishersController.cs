using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Publishers.Application.UseCases;

namespace WebRouterApp.Features.Publishers.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PublishersController : Controller
    {
        private readonly EndpointHandler<PublishersController> _endpointHandler;

        public PublishersController(EndpointHandler<PublishersController> endpointHandler)
        {
            _endpointHandler = endpointHandler;
        }

        [HttpPost]
        public Task<IActionResult> GetOverviews([FromBody] PublisherOverviewsApiQuery query) =>
            _endpointHandler.Handle(query);

        [HttpPost]
        public Task<IActionResult> GetRecords([FromBody] PublisherRecordsApiQuery query) =>
            _endpointHandler.Handle(query);

        [HttpPost]
        public Task<IActionResult> Update([FromBody] UpdatePublisherApiCommand command) =>
            _endpointHandler.Handle(command);
    }
}
