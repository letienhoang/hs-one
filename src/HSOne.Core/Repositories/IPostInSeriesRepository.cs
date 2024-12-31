﻿using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface IPostInSeriesRepository : IRepository<PostInSeries, Guid>
    {
        Task<bool> IsPostInSeriesAsync(Guid seriesId, Guid postId);
        Task AddPostToSeriesAsync(Guid seriesId, Guid postId, int sortOrder);
        Task RemovePostToSeriesAsync(Guid seriesId, Guid postId);
        Task<PostInSeriesDto> GetPostInSeriesAsync(Guid postId, Guid seriesId);
        Task<bool> HasPostsInSeriesAsync(Guid seriesId);
    }
}
