using System.Text.Json.Serialization;
using WebRouterApp.Features.Auth.Application.Models;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts
{
    public abstract class RefreshTokenApiResponseBase
    {
        protected RefreshTokenApiResponseBase(RefreshToken refreshToken)
        {
            RefreshToken = refreshToken;
        }

        // We return refresh tokens in an http-only cookie.
        [JsonIgnore] 
        public RefreshToken RefreshToken { get; }
    }
}
