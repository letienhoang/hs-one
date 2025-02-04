using HSOne.Core.Domain.Identity;
using HSOne.Core.Repositories;
using HSOne.Data.SeedWorks;

namespace HSOne.Data.Repositories
{
    public class UserRepository : RepositoryBase<AppUser, Guid>, IUserRepository
    {
        public UserRepository(HSOneContext context) : base(context)
        {
        }
    }
}
