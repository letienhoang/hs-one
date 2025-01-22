using HSOne.Core.Domain.Identity;
using HSOne.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace HSOne.WebApp.Helpers
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        public CustomClaimsPrincipalFactory(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(UserClaims.UserId, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(UserClaims.FirstName, user.FirstName),
            });
            return principal;
        }
    }
}
