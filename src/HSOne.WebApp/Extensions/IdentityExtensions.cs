using HSOne.Core.SeedWorks.Constants;
using System.Security.Claims;

namespace HSOne.WebApp.Extensions
{
    public static class IdentityExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var subjectId = claimsPrincipal.GetSpecialClaim(ClaimTypes.NameIdentifier);
            return Guid.Parse(subjectId);
        }

        public static string GetSpecialClaim(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = (claimsPrincipal.Identity as ClaimsIdentity).Claims.FirstOrDefault(x => x.Type == claimType);

            return claim != null ? claim.Value : string.Empty;
        }

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetSpecialClaim(ClaimTypes.Name);
        }

        public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetSpecialClaim(UserClaims.FirstName);
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetSpecialClaim(UserClaims.Email);
        }
    }
}
