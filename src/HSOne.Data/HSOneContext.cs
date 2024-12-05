using HSOne.Core.Domain.Content;
using HSOne.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data
{
    public class HSOneContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public HSOneContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostActivityLog> PostActivityLogs { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostInSeries> PostInSeries { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims").HasKey(x => x.Id);
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);
        }

        // HS.UIF (Update in future) - Add DateCreated and ModifiedDate to all entities
        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        //{
        //    var entries = ChangeTracker
        //        .Entries()
        //        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        //    foreach (var entityEntry in entries)
        //    {
        //        var dateCreatedProp = entityEntry.Entity.GetType().GetProperty("DateCreated");
        //        if (entityEntry.State == EntityState.Added
        //            && dateCreatedProp != null)
        //        {
        //            dateCreatedProp.SetValue(entityEntry.Entity, DateTime.Now);
        //        }
        //        var modifiedDateProp = entityEntry.Entity.GetType().GetProperty("ModifiedDate");
        //        if (entityEntry.State == EntityState.Modified
        //            && modifiedDateProp != null)
        //        {
        //            modifiedDateProp.SetValue(entityEntry.Entity, DateTime.Now);
        //        }
        //    }
        //    return base.SaveChangesAsync(cancellationToken);
        //}
    }
}