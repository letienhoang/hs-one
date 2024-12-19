using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Repositories;
using HSOne.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data.Repositories
{
    public class PostInSeriesRepository : RepositoryBase<PostInSeries, Guid>, IPostInSeriesRepository
    {
        private readonly IMapper _mapper;
        public PostInSeriesRepository(HSOneContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<bool> IsPostInSeriesAsync(Guid seriesId, Guid postId)
        {
            return await _context.PostInSeries.AnyAsync(x => x.SeriesId == seriesId && x.PostId == postId);
        }
    }
}
