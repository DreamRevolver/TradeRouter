using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.Validation;
using WebRouterApp.Features.Auth.Application.ErrorParts;

namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts
{
    public sealed class EndpointHandler<TController>
        where TController : ControllerBase
    {
        private readonly ApiRequestDispatcher _apiRequestDispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TController> _log;
        private readonly ValidatorFactory _validatorFactory;

        public EndpointHandler(
            ILogger<TController> log,
            ValidatorFactory validatorFactory,
            ApiRequestDispatcher apiRequestDispatcher,
            IHttpContextAccessor httpContextAccessor)
        {
            _log = log;
            _validatorFactory = validatorFactory;
            _apiRequestDispatcher = apiRequestDispatcher;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Handle<TApiResponse>(IApiRequest<TApiResponse> request)
        {
            try
            {
                var validationError = Validate(request);
                if (validationError != null)
                    // 400: Validation failed.
                    return new BadRequestObjectResult(validationError);

                var response = await _apiRequestDispatcher.Send(request);

                if (response is RefreshTokenApiResponseBase refreshTokenApiResponseBase)
                {
                    var refreshToken = refreshTokenApiResponseBase.RefreshToken;
                    // Append a cookie with the new refresh token to the http response.
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = refreshToken.Expires
                    };

                    _httpContextAccessor
                        .HttpContext?
                        .Response
                        .Cookies
                        .Append(CookieNames.RefreshToken, refreshToken.Token, cookieOptions);
                }

                // 200: OK
                return new OkObjectResult(response);
            }
            catch (UnauthorizedException ex)
            {
                // An attempt to use an API method without proper authorization encountered.
                _log.LogError(ex, "An unauthorized error encountered");

                // 401: A log in attempt failed.
                // Look to the JSON response body for the specifics of the error.
                return new UnauthorizedObjectResult(ex.Error);
            }
            catch (EndpointException ex)
            {
                // An error specific to this particular API method encountered.
                _log.LogError(ex, "An endpoint-specific error encountered");

                // 409: Endpoint-specific error.
                // Look to the JSON response body for the specifics of the error.
                return new ConflictObjectResult(ex.Error);
            }
        }

        private ValidationError? Validate<TRequest>(TRequest request)
        {
            var validator = _validatorFactory.ValidatorFor<TRequest>();
            if (validator == null)
                return null;

            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
                return null;

            var propertyErrors = new Dictionary<string, PropertyValidationError>();
            foreach (var error in (IEnumerable<ValidationFailure>)validationResult.Errors)
            {
                if (!propertyErrors.TryGetValue(error.PropertyName, out var propertyError))
                {
                    propertyError = new PropertyValidationError(
                        error.PropertyName,
                        error.AttemptedValue?.ToString() ?? "");

                    propertyErrors.Add(propertyError.PropertyName, propertyError);
                }

                propertyError.AddError(error.ErrorMessage);
            }

            return new ValidationError(propertyErrors.Values);
        }
    }
}
