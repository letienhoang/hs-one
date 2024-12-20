using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface ISeriesRepository : IRepository<Series, Guid>
    {
        Task<List<Series>> GetPopularSeriesAsync(int count);
        Task<PagedResult<SeriesInListDto>> GetSeriesPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10);
    }
}