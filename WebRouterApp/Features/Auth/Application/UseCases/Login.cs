using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Options;
using WebRouterApp.Core.CopyEngineParts;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Features.Auth.Application.ErrorParts;
using WebRouterApp.Features.Auth.Application.Models;
using WebRouterApp.Features.Auth.Application.Services;
using WebRouterApp.Features.Users.Application.Models;
using WebRouterApp.Shared;

namespace WebRouterApp.Features.Auth.Application.UseCases
{
    public sealed record LoginApiCommand(string? Username, string? Password) : IApiRequest<LoginApiResponse>;

    public sealed class LoginApiResponse : RefreshTokenApiResponseBase
    {
        public LoginApiResponse(UserModel user, string accessToken, RefreshToken refreshToken) 
            : base(refreshToken)
        {
            User = user;
            AccessToken = accessToken;
        }

        public UserModel User { get; }
        public string AccessToken { get; }
    }
    
    public sealed class LoginApiCommandValidator : AbstractValidator<LoginApiCommand>
    {
        public LoginApiCommandValidator() 
        {
            RuleFor(it => it.Username).Must(username => !string.IsNullOrWhiteSpace(username));
            RuleFor(it => it.Password).Must(password => !string.IsNullOrWhiteSpace(password));
        }
    }
    
    public class LoginApiCommandHandler : IApiRequestHandler<LoginApiCommand, LoginApiResponse>
    {
        private readonly JwtGenerator _jwtGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly TradeRouterDbContext _dbContext;
        private readonly RefreshTokenOptions _refreshTokenOptions;
        private readonly CopyEngine _copyEngine;

        public LoginApiCommandHandler(
            JwtGenerator jwtGenerator, 
            TradeRouterDbContext dbContext, 
            RefreshTokenGenerator refreshTokenGenerator, 
            IOptions<RefreshTokenOptions> refreshTokenOptions,
            CopyEngine copyEngine)
        {
            _jwtGenerator = jwtGenerator;
            _dbContext = dbContext;
            _refreshTokenGenerator = refreshTokenGenerator;
            _copyEngine = copyEngine;
            _refreshTokenOptions = refreshTokenOptions.Value;
        }

        public async Task<LoginApiResponse> Handle(LoginApiCommand command)
        {
            CodeAsserts.NotNull(command.Username);
            CodeAsserts.NotNull(command.Password);

            var user = _dbContext.Users.SingleOrDefault(it => it.Username == command.Username) 
                       ?? throw new UnauthorizedException(new UnauthorizedError.InvalidCredentials());

            byte[] passwordHash;
            using (var hasher = new PasswordHasher())
                passwordHash = await hasher.HashAsync(command.Password, user.Salt);

            if (!passwordHash.SequenceEqual(user.PasswordHash))
                throw new UnauthorizedException(new UnauthorizedError.InvalidCredentials());

            var userModel = new UserModel(
                Id: user.Id,
                Username: user.Username, 
                FirstName: user.FirstName,
                LastName: user.LastName);

            var jwt = _jwtGenerator.ForSubject(userModel.Id.ToString());
            var rt = _refreshTokenGenerator.RefreshToken();
            
            user.RemoveInactiveRefreshTokens(olderThan: _refreshTokenOptions.TimeToLiveForInactive, DateTime.UtcNow);
            user.RefreshTokens.Add(rt);
            
            await _dbContext.SaveChangesAsync();
            
            await _copyEngine.EnsureSessionCreated(userModel.Id);

            return new LoginApiResponse(userModel, accessToken: jwt, refreshToken: rt);
        }
    }
    
    public sealed class PasswordHasher : IDisposable
    {
        private readonly HMACSHA512 _hashAlgorithm = new(Encoding.UTF8.GetBytes("traderouter@tradingforge"));

        public Task<byte[]> HashAsync(string password, byte[] saltBytes)
        {
            var saltedPasswordBytes = GetSaltedPasswordBytes(password, saltBytes);
            return _hashAlgorithm.ComputeHashAsync(new MemoryStream(saltedPasswordBytes));
        }

        public byte[] Hash(string password, byte[] saltBytes)
        {
            var saltedPasswordBytes = GetSaltedPasswordBytes(password, saltBytes);
            return _hashAlgorithm.ComputeHash(new MemoryStream(saltedPasswordBytes));
        }

        private static byte[] GetSaltedPasswordBytes(string password, byte[] saltBytes)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var saltedPasswordBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(src: passwordBytes, srcOffset: 0, dst: saltedPasswordBytes, dstOffset: 0, count: passwordBytes.Length);
            Buffer.BlockCopy(src: saltBytes, srcOffset: 0, dst: saltedPasswordBytes, dstOffset: passwordBytes.Length, count: saltBytes.Length);

            return saltedPasswordBytes;
        }

        public void Dispose() => _hashAlgorithm.Dispose();
    }

}
