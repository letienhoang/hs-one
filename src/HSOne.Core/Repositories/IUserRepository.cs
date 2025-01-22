using HSOne.Core.Domain.Identity;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface IUserRepository : IRepository<AppUser, Guid>
    {
    }
}
