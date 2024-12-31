using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<List<Post>> GetPopularPostsAsync(int count);
        Task<PagedResult<PostInListDto>> GetPostsPagingAsync(string? keyword, Guid userId, Guid? categoryId, int pageIndex = 1, int pageSize = 10);
        Task<List<PostInListDto>> GetAllPostsInSeriesAsync(Guid seriesId);
        Task<bool> IsSlugAlreadyExistedAsync(string slug, Guid? postId = null);
        Task ApproveAsync(Guid id, Guid userId);
        Task SendForApprovalAsync(Guid id, Guid userId);
        Task RejectAsync(Guid id, Guid userId, string note);
        Task BackToDraftAsync(Guid id, Guid userId);
        Task<string> GetRejectReasonAsync(Guid id);
        Task<bool> HasPublishInLastAsync(Guid id);
        Task<List<PostActivityLogDto>> GetActivityLogsAsync(Guid id);
        Task<List<Post>> GetUnpaidPublishPostsAsync(Guid userId);
        Task<bool> HasPostsInCategoryAsync(Guid categoryId);
    }
}