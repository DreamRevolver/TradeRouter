using System;
using System.Security.Cryptography;
using WebRouterApp.Features.Auth.Application.Models;

namespace WebRouterApp.Features.Auth.Application.Services
{
    public sealed class RefreshTokenGenerator
    {
        public RefreshToken RefreshToken()
        {
            var randomBytes = new byte[24];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(randomBytes);

            var refreshToken = new RefreshToken(
                token: Convert.ToBase64String(randomBytes),
                created: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(7)
            );

            return refreshToken;
        }
    }
    
    public sealed class RefreshTokenOptions
    {
        public TimeSpan TimeToLiveForInactive { get; set; } = TimeSpan.FromDays(2);
    }
}
