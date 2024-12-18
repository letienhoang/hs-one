using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface IPostCategoryRepository : IRepository<PostCategory, Guid>
    {
        Task<List<PostCategory>> GetPopularPostCategoriesAsync(int count);

        Task<PagedResult<PostCategoryDto>> GetPostCategoriesPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10);
    }
}