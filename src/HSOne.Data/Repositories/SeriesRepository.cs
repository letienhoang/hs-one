using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.Repositories;
using HSOne.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data.Repositories
{
    public class SeriesRepository : RepositoryBase<Series, Guid>, ISeriesRepository
    {
        private readonly IMapper _mapper;
        public SeriesRepository(HSOneContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<Series>> GetPopularSeriesAsync(int count)
        {
            return await _context.Series.OrderByDescending(x => x.DateCreated).Take(count).ToListAsync();
        }

        public async Task<PagedResult<SeriesInListDto>> GetSeriesPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Series.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }

            var totalRecords = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<SeriesInListDto>
            {
                Results = await _mapper.ProjectTo<SeriesInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                PageSize = pageSize,
                RowCount = totalRecords
            };
        }

        public async Task<List<SeriesInListDto>> GetAllSeriesForPostAsync(Guid postId)
        {
            var query = from pis in _context.PostInSeries
                        join s in _context.Series on pis.SeriesId equals s.Id
                        where pis.PostId == postId
                        select s;
            return await _mapper.ProjectTo<SeriesInListDto>(query).ToListAsync();
        }

        public async Task<SeriesDto> GetSeriesBySlugAsync(string slug)
        {
            var series = await _context.Series.Where(x => x.Slug == slug).FirstOrDefaultAsync();
            return  _mapper.Map<SeriesDto>(series);
        }
    }
}
