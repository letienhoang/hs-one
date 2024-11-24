using HSOne.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace HSOne.Data
{
    public class DataSeeder
    {
        public async Task SeedAsync(HSOneContext context)
        {
            var rootAdminRoleId = Guid.NewGuid();
            if (!context.Roles.Any())
            {
                await context.Roles.AddAsync(new AppRole()
                {
                    Id = rootAdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    DisplayName = "Administrator",
                });
                await context.SaveChangesAsync();
            }

            var passwordHasher = new PasswordHasher<AppUser>();
            if (!context.Users.Any())
            {
                var rootAdminId = Guid.NewGuid();
                var rootAdmin = new AppUser()
                {
                    Id = rootAdminId,
                    FirstName = "Holwn",
                    LastName = "Admin",
                    UserName = "HolwnAdmin",
                    NormalizedUserName = "HOLWNADMIN",
                    Email = "admin@hoctat.com",
                    NormalizedEmail = "ADMIN@HOCTAT.COM",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                    DateCreated = DateTime.Now,
                };
                rootAdmin.PasswordHash = passwordHasher.HashPassword(rootAdmin, "Admin@123");
                await context.Users.AddAsync(rootAdmin);

                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>()
                {
                    RoleId = rootAdminRoleId,
                    UserId = rootAdminId,
                });

                await context.SaveChangesAsync();
            }
        }
    }
}