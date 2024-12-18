using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.Repositories;
using HSOne.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data.Repositories
{
    public class PostCategoryRepository : RepositoryBase<PostCategory, Guid>, IPostCategoryRepository
    {
        private readonly IMapper _mapper;
        public PostCategoryRepository(HSOneContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public Task<List<PostCategory>> GetPopularPostCategoriesAsync(int count)
        {
            return _context.PostCategories.OrderByDescending(x => x.SortOrder).Take(count).ToListAsync();
        }

        public async Task<PagedResult<PostCategoryDto>> GetPostCategoriesPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.PostCategories.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            var totalRows = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<PostCategoryDto>
            {
                Results = await _mapper.ProjectTo<PostCategoryDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRows,
                PageSize = pageSize
            };
        }
    }
}
