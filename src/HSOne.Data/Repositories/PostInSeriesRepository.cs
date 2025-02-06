using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;
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
            return await _context.PostInSeries.AnyAsync(x => x.PostId == postId && x.SeriesId == seriesId);
        }

        public async Task<bool> HasPostsInSeriesAsync(Guid seriesId)
        {
            return await _context.PostInSeries.AnyAsync(x => x.SeriesId == seriesId);
        }

        public async Task<bool> HasSeriesContainingPostAsync(Guid postId)
        {
            return await _context.PostInSeries.AnyAsync(x => x.PostId == postId);
        }

        public async Task<PostInSeriesDto> GetPostInSeriesAsync(Guid postId, Guid seriesId)
        {
            var postInSeries = await _context.PostInSeries.FirstOrDefaultAsync(x => x.SeriesId == seriesId && x.PostId == postId);
            return _mapper.Map<PostInSeriesDto>(postInSeries);
        }

        public async Task AddPostToSeriesAsync(Guid seriesId, Guid postId, int sortOrder)
        {
            var postInSeries = await _context.PostInSeries.FirstOrDefaultAsync(x => x.SeriesId == seriesId && x.PostId == postId);
            if (postInSeries == null)
            {
                await _context.PostInSeries.AddAsync(new PostInSeries()
                {
                    SeriesId = seriesId,
                    PostId = postId,
                    DisplayOrder = sortOrder
                });
            }
        }

        public async Task RemovePostToSeriesAsync(Guid seriesId, Guid postId)
        {
            var postInSeries = await _context.PostInSeries.FirstOrDefaultAsync(x => x.SeriesId == seriesId && x.PostId == postId);
            if (postInSeries != null)
            {
                _context.PostInSeries.Remove(postInSeries);
            }
        }

        public async Task RemovePostInAllSeriesAsync(Guid postId)
        {
            var postInSeries = await _context.PostInSeries.Where(x => x.PostId == postId).ToListAsync();
            if (postInSeries.Any())
            {
                _context.PostInSeries.RemoveRange(postInSeries);
            }
        }

        public async Task RemoveLinkToSeriesAsync(Guid seriesId)
        {
            var postInSeries = await _context.PostInSeries.Where(x => x.SeriesId == seriesId).ToListAsync();
            if (postInSeries.Any())
            {
                _context.PostInSeries.RemoveRange(postInSeries);
            }
        }
    }
}
