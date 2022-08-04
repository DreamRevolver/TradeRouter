using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Logs.Application.UseCases;

namespace WebRouterApp.Features.Logs.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class LogsController : Controller
    {
        private readonly EndpointHandler<LogsController> _endpointHandler;

        public LogsController(EndpointHandler<LogsController> endpointHandler)
        {
            _endpointHandler = endpointHandler;
        }

        [HttpPost]
        public Task<IActionResult> Logs([FromBody] LogsApiQuery query)
            => _endpointHandler.Handle(query);

    }

}
