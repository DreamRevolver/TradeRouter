using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Auth.Application.UseCases;

namespace WebRouterApp.Features.Auth.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly EndpointHandler<AuthController> _endpointHandler;
        
        public AuthController(EndpointHandler<AuthController> endpointHandler)
        {
            _endpointHandler = endpointHandler;
        }

        [HttpPost]
        [AllowAnonymous]
        public Task<IActionResult> Login([FromBody] LoginApiCommand command) => _endpointHandler.Handle(command);

        [HttpPost]
        [AllowAnonymous]
        public Task<IActionResult> Refresh([FromBody] RefreshTokenApiCommand command) 
            => _endpointHandler.Handle(command with { RefreshToken = Request.Cookies[CookieNames.RefreshToken] });
    }
}
