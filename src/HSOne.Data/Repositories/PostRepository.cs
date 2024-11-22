using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Repositories;
using HSOne.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
    {
        public PostRepository(HSOneContext context) : base(context)
        {
        }

        public Task<List<Post>> GetPopularPostsAsync(int count)
        {
            return _context.Posts.OrderByDescending(x => x.ViewCount).Take(count).ToListAsync();
        }

        public async Task<PagedResult<Post>> GetPostsPagingAsync(string keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            var totalRecords = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<Post>
            {
                Results = await query.ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
        }
    }
}