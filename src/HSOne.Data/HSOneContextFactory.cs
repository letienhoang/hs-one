using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HSOne.Data
{
    public class HSOneContextFactory : IDesignTimeDbContextFactory<HSOneContext>
    {
        public HSOneContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<HSOneContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new HSOneContext(builder.Options);
        }
    }
}