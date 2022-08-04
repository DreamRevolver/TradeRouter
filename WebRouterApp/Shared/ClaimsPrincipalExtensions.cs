using System;
using System.ComponentModel;
using System.Security.Claims;

namespace WebRouterApp.Shared
{
    public static class ClaimsPrincipalExtensions
    {
        public static T? Id<T>(this ClaimsPrincipal principal)
        {
            var serializedId = principal.First(ClaimTypes.NameIdentifier);
            if (serializedId == null)
                return default;

            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(serializedId);
        }

        public static string? Name(this ClaimsPrincipal principal) => principal.First(ClaimTypes.Name);
        public static string? Email(this ClaimsPrincipal principal) => principal.First(ClaimTypes.Email);

        private static string? First(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(claimType);
        }
    }
}