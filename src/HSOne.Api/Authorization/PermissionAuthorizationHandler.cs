using HSOne.Core.Domain.Identity;
using HSOne.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HSOne.Api.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public PermissionAuthorizationHandler(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {   
            
            if (context.User.Identity.IsAuthenticated == false)
            {
                return;
            }
            
            var user = await _userManager.FindByNameAsync(context.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            if ( roles.Contains(Roles.Admin))
            {
                context.Succeed(requirement);
                return;
            }
            var allPermissions = new List<Claim>();
            foreach (var role in roles) {
                var appRole = await _roleManager.FindByNameAsync(role);
                IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(appRole);
                allPermissions.AddRange(roleClaims);
            }

            var permissions = allPermissions.Where(x => x.Type == "Permissions" && x.Value == requirement.Permission && x.Issuer == "LOCAL AUTHORITY");

            if (permissions.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
