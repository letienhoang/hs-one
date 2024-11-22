using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<List<Post>> GetPopularPostsAsync(int count);
        Task<PagedResult<Post>> GetPostsPagingAsync(string keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10);
    }
}