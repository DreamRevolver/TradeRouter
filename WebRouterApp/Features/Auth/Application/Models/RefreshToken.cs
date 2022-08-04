using System;
using System.Text.Json.Serialization;
using WebRouterApp.Core.Application.Models;

namespace WebRouterApp.Features.Auth.Application.Models
{
    public class RefreshToken : IEntity
    {
        public RefreshToken(string token, DateTime created, DateTime expires)
        {
            Token = token;
            Created = created;
            Expires = expires;
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        public string Token { get; private set; }
        public  DateTime Created { get; private set; }
        public DateTime Expires { get; private set; }
        
        public DateTime? Revoked { get; private set; }
        public string? ReplacedBy { get; private set; }
        
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        
        public bool IsActive => !IsRevoked && !IsExpired;
        
        public void Revoke(RefreshToken? replacedBy, DateTime revokedAt)
        {
            Revoked = revokedAt;
            ReplacedBy = replacedBy?.Token;
        }
    }
}