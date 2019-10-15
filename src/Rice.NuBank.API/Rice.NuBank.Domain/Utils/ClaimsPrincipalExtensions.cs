using System.Security.Claims;

namespace Rice.NuBank.Domain.Utils
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetClaim(this ClaimsPrincipal user, string claimName)
        {
            return user.FindFirst(claimName)?.Value;
        }

        public static string GetQrCode(this ClaimsPrincipal user)
        {
            return user.GetClaim("qr-code");
        }
    }
}