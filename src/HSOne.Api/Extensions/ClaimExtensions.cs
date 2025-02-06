using HSOne.Core.Domain.Identity;
using HSOne.Core.Models.System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

namespace HSOne.Api.Extensions
{
    public static class ClaimExtensions
    {
        public static void GetPermissions(this List<RoleClaimsDto> allPermissions, Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                var attr = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                string displayName = field.GetValue(null).ToString();
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attributes.Length > 0)
                {
                    var description = (DescriptionAttribute)attributes[0];
                    displayName = description.Description;
                }
                allPermissions.Add(new RoleClaimsDto
                {
                    Type = "Permissions",
                    Value = field.GetValue(null).ToString(),
                    DisplayName = displayName
                });
            }
        }

        public static async Task AddPermissionClaimAsync(this RoleManager<AppRole> roleManager, AppRole role, string permission)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            if (!claims.Any(x => x.Type == "Permissions" && x.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permissions", permission));
            }
        }
    }
}
