using HSOne.Core.Domain.Content;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<List<Post>> GetPopularPostsAsync(int count);
    }
}