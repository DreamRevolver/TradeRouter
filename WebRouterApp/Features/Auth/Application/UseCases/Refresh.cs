using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Auth.Application.ErrorParts;
using WebRouterApp.Features.Auth.Application.Models;
using WebRouterApp.Features.Auth.Application.Services;
using WebRouterApp.Features.Users.Application.Models;
using WebRouterApp.Shared;

namespace WebRouterApp.Features.Auth.Application.UseCases
{
    public sealed record RefreshTokenApiCommand(string? RefreshToken) : IApiRequest<RefreshTokenApiResponse>;

    public sealed class RefreshTokenApiResponse : RefreshTokenApiResponseBase
    {
        public RefreshTokenApiResponse(UserModel user, string accessToken, RefreshToken refreshToken) 
            : base(refreshToken)
        {
            User = user;
            AccessToken = accessToken;
        }

        public UserModel User { get; }
        public string AccessToken { get; }
    }
    
    public sealed class RefreshTokenApiCommandValidator : AbstractValidator<RefreshTokenApiCommand>
    {
        public RefreshTokenApiCommandValidator() 
        {
            RuleFor(it => it.RefreshToken).Must(value => !string.IsNullOrWhiteSpace(value));
        }
    }
    
    public sealed class RefreshTokenApiCommandHandler : IApiRequestHandler<RefreshTokenApiCommand, RefreshTokenApiResponse>
    {
        private readonly JwtGenerator _jwtGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly TradeRouterDbContext _dbContext;
        private readonly RefreshTokenOptions _refreshTokenOptions;

        public RefreshTokenApiCommandHandler(
            JwtGenerator jwtGenerator, 
            TradeRouterDbContext dbContext, 
            RefreshTokenGenerator refreshTokenGenerator,
            IOptions<RefreshTokenOptions> refreshTokenOptions)
        {
            _jwtGenerator = jwtGenerator;
            _dbContext = dbContext;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenOptions = refreshTokenOptions.Value;
        }

        public Task<RefreshTokenApiResponse> Handle(RefreshTokenApiCommand command)
        {
            CodeAsserts.NotNull(command.RefreshToken);
            
            var user = _dbContext.Users
                           .Include(u => u.RefreshTokens)
                           .SingleOrDefault(u => u.RefreshTokens
                               .Any(it => it.Token == command.RefreshToken));
            
            if (user == null)
                throw new UnauthorizedException(new UnauthorizedError.InvalidToken());

            var refreshToken = user.GetRequiredRefreshToken(command.RefreshToken);
            if (refreshToken.IsRevoked)
            {
                // We revoke a refresh token, when the user exchanges it for a fresh access token.
                // In the course of exchange we:
                // - Generate a fresh access token
                // - Rotate the refresh token: generate a new one, revoke and replace the current one.
                //
                // Otherwise, this refresh token could've been directly revoked.
                //
                // In any case, an attempt to use a revoked refresh token means it has been compromised.
                // We need to revoke the active descendant as it may be in the hands of a malicious actor,
                // if they managed to exchange the compromised refresh token sooner than the legitimate user.
                user.RevokeDescendantsOf(refreshToken, revokedAt: DateTime.UtcNow);
                
                _dbContext.Update(user);
                _dbContext.SaveChanges();
            }

            if (!refreshToken.IsActive)
                throw new UnauthorizedException(new UnauthorizedError.InvalidToken());

            // Rotate the refresh token: replace the current with a new one.
            var newRefreshToken = _refreshTokenGenerator.RefreshToken();
            user.Rotate(currentToken: refreshToken, newToken: newRefreshToken, rotatedAt: DateTime.UtcNow);

            user.RemoveInactiveRefreshTokens(olderThan: _refreshTokenOptions.TimeToLiveForInactive, DateTime.UtcNow);

            _dbContext.Update(user);
            _dbContext.SaveChanges();

            var userModel = new UserModel(
                Id: user.Id,
                Username: user.Username, 
                FirstName: user.FirstName,
                LastName: user.LastName);

            return Task.FromResult(
                new RefreshTokenApiResponse(
                    user: userModel,
                    accessToken: _jwtGenerator.ForSubject(userModel.Id.ToString()), 
                    refreshToken: newRefreshToken
                )
            );
        }
    }
}
