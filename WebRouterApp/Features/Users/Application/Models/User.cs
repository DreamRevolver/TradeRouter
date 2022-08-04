using System;
using System.Collections.Generic;
using System.Linq;
using WebRouterApp.Core.Application.Models;
using WebRouterApp.Features.Auth.Application.Models;

namespace WebRouterApp.Features.Users.Application.Models
{
    public record UserModel(Guid Id, string Username, string FirstName, string LastName);

    public class UserRecord  : IEntity
    {
        public UserRecord(string username, string firstName, string lastName, byte[] passwordHash, byte[] salt)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
            Salt = salt;
            RefreshTokens = new List<RefreshToken>();
        }

        public Guid Id { get; set; }
        
        public string Username { get; private set; }
        
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        
        public byte[] PasswordHash { get; private set; }
        public byte[] Salt { get; private set; } 
        
        public List<RefreshToken> RefreshTokens { get; private set; }

        public RefreshToken GetRequiredRefreshToken(string refreshToken) => 
            RefreshTokens.First(it => it.Token == refreshToken);

        public void Rotate(RefreshToken currentToken, RefreshToken newToken, DateTime rotatedAt)
        {
            currentToken.Revoke(revokedAt: rotatedAt, replacedBy: newToken);
            RefreshTokens.Add(newToken);
        }

        public void RevokeDescendantsOf(RefreshToken ancestorToken, DateTime revokedAt)
        {
            // Walk the refresh tokens ancestry chain up to the active descendant.
            // If it exists, revoke.
            if (string.IsNullOrWhiteSpace(ancestorToken.ReplacedBy)) 
                return;
            
            var nextToken = RefreshTokens.First(it => ancestorToken.ReplacedBy == it.Token);
            if (!nextToken.IsActive)
                RevokeDescendantsOf(nextToken, revokedAt);

            nextToken.Revoke(replacedBy: null, revokedAt);
        }

        public void RemoveInactiveRefreshTokens(TimeSpan olderThan, DateTime utcNow)
        {
            RefreshTokens.RemoveAll(it => !it.IsActive && it.Created.Add(olderThan) <= utcNow);
        }
    }
}
