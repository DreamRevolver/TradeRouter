using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WebRouterApp.Features.Auth.Application.Services
{
    public sealed class JwtGenerator
    {
        private readonly JwtOptions _options;

        public JwtGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _options = jwtOptions.Value;
        }

        public string ForSubject(string subject)
        {
            var issuedAt = DateTime.UtcNow;
            var iat = new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, iat, ClaimValueTypes.Integer64)
            };
            
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: issuedAt,
                expires: issuedAt.Add(_options.ValidFor),
                signingCredentials: _options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

    }
        
    public sealed class JwtOptions
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(5);
        public SigningCredentials? SigningCredentials { get; set; }
    }
}
