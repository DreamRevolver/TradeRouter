using System;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts;

namespace WebRouterApp.Features.Auth.Application.ErrorParts
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(UnauthorizedError error)
        {
            Error = error;
        }

        public UnauthorizedError Error { get; }
    }

    public abstract class UnauthorizedError
    {
        private UnauthorizedError() {}
        
        public sealed class InvalidCredentials : UnauthorizedError, IApiError
        {
            public string Tag => ErrorTags.InvalidCredentialsError;
            public string Message => "Invalid username or password.";
        }
        
        public sealed class InvalidToken : UnauthorizedError, IApiError
        {
            public string Tag => ErrorTags.InvalidTokenError;
            public string Message => "Invalid token.";
        }
    }
}
