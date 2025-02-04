using AutoMapper;
using Azure;
using HSOne.Core.Domain.Content;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.Repositories;
using HSOne.Core.SeedWorks.Constants;
using HSOne.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public PostRepository(HSOneContext context, IMapper mapper, UserManager<AppUser> userManager) : base(context)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public Task<List<Post>> GetPopularPostsAsync(int count)
        {
            return _context.Posts.OrderByDescending(x => x.ViewCount).Take(count).ToListAsync();
        }

        public async Task<PagedResult<PostInListDto>> GetPostsPagingAsync(string? keyword, Guid userId, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var canApprove = false;
            if (roles.Contains(Roles.Admin))
            {
                canApprove = true;
            }
            else
            {
                canApprove = await _context.RoleClaims.AnyAsync(x => roles.Contains(x.RoleId.ToString()) && x.ClaimValue == Permissions.Posts.Approve);
            }

            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            if (!canApprove)
            {
                query = query.Where(x => x.AuthorUserId == userId);
            }

            var totalRecords = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
        }

        public async Task<List<PostInListDto>> GetAllPostsInSeriesAsync(Guid seriesId)
        {
            var query = from pis in _context.PostInSeries
                        join p in _context.Posts on pis.PostId equals p.Id
                        where pis.SeriesId == seriesId
                        select p;
            return await _mapper.ProjectTo<PostInListDto>(query).ToListAsync();
        }

        public Task<bool> IsSlugAlreadyExistedAsync(string slug, Guid? postId = null)
        {
            if (postId.HasValue)
            {
                return _context.Posts.AnyAsync(x => x.Slug == slug && x.Id != postId);
            }
            return _context.Posts.AnyAsync(x => x.Slug == slug);
        }

        public async Task ApproveAsync(Guid id, Guid userId)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Post does not exist");
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("User does not exist");
            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.Published,
                PostId = id,
                UserId = userId,
                UserName = user.UserName!,
                Note = $"{user.UserName} approved the post"
            });

            post.Status = PostStatus.Published;
            _context.Posts.Update(post);
        }

        public async Task SendForApprovalAsync(Guid id, Guid userId)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Post does not exist");
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("User does not exist");

            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.WaitingForApproval,
                PostId = id,
                UserId = userId,
                UserName = user.UserName!,
                Note = $"{user.UserName} sent the post for approval"
            });

            post.Status = PostStatus.WaitingForApproval;
            _context.Posts.Update(post);
        }

        public async Task RejectAsync(Guid id, Guid userId, string note)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Post does not exist");
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("User does not exist");

            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.Rejected,
                PostId = id,
                UserId = userId,
                UserName = user.UserName!,
                Note = $"{user.UserName} returned the post with note: {note}"
            });

            post.Status = PostStatus.Rejected;
            _context.Posts.Update(post);
        }

        public async Task<string> GetRejectReasonAsync(Guid id)
        {
            var activityLog = await _context.PostActivityLogs
                .Where(x => x.PostId == id && x.ToStatus == PostStatus.Rejected)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefaultAsync();
            return activityLog?.Note ?? string.Empty;
        }

        public async Task<bool> HasPublishInLastAsync(Guid id)
        {
            var hasPublished = await _context.PostActivityLogs
                .CountAsync(x => x.PostId == id && x.ToStatus == PostStatus.Published);
            return hasPublished > 0;
        }

        public async Task<List<PostActivityLogDto>> GetActivityLogsAsync(Guid id)
        {
            var query = _context.PostActivityLogs.Where(x => x.PostId == id)
                .OrderByDescending(x => x.DateCreated);
            return await _mapper.ProjectTo<PostActivityLogDto>(query).ToListAsync();
        }

        public async Task BackToDraftAsync(Guid id, Guid userId)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Post does not exist");
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("User does not exist");

            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.Draft,
                PostId = id,
                UserId = userId,
                UserName = user.UserName!,
                Note = $"{user.UserName} return back to draft"
            });

            post.Status = PostStatus.Draft;
            _context.Posts.Update(post);
        }

        public async Task<List<Post>> GetUnpaidPublishPostsAsync(Guid userId)
        {
            return await _context.Posts.Where(x => x.AuthorUserId == userId && x.Status == PostStatus.Published && !x.IsPaid).ToListAsync();
        }

        public async Task<bool> HasPostsInCategoryAsync(Guid categoryId)
        {
            return await _context.Posts.AnyAsync(x => x.CategoryId == categoryId);
        }

        public async Task<List<PostInListDto>> GetLatestPublishPostsAsync(int count)
        {
            var query = _context.Posts.Where(x => x.Status == PostStatus.Published)
                .Take(count)
                .OrderByDescending(x => x.DateCreated);
            return await _mapper.ProjectTo<PostInListDto>(query).ToListAsync();
        }

        public async Task<PagedResult<PostInListDto>> GetPostsByCategoryPagingAsync(string categortSlug, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(categortSlug))
            {
                query = query.Where(x => x.CategorySlug == categortSlug);
            }

            var totalRecords = await query.CountAsync();

            query = query.Where(x => x.Status == PostStatus.Published)
                .OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
        }

        public async Task<PostDto> GetBySlugAsync(string slug)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Slug == slug);
            return post == null ? throw new Exception($"Cannot find post with Slug: {slug}") : _mapper.Map<PostDto>(post);
        }

        public async Task<PagedResult<PostInListDto>> GetPostsByTagPagingAsync(string tagSlug, int pageIndex = 1, int pageSize = 10)
        {
            var query = from p in _context.Posts
                        join pt in _context.PostTags on p.Id equals pt.PostId
                        join t in _context.Tags on pt.TagId equals t.Id
                        where t.Slug == tagSlug
                        select p;

            var totalRecords = await query.CountAsync();

            query = query.Where(x => x.Status == PostStatus.Published)
                .OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<PostInListDto>> GetAllPostBySeriesSlugPagingAsync(string seriesSlug, int pageIndex = 1, int pageSize = 10)
        {
            var query = from p in _context.Posts
                        join ps in _context.PostInSeries on p.Id equals ps.PostId
                        join s in _context.Series on ps.SeriesId equals s.Id
                        where s.Slug == seriesSlug
                        select p;

            var totalRecords = await query.CountAsync();

            query = query.Where(x => x.Status == PostStatus.Published)
                .OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<PostInListDto>> GetPostsByUserPagingAsync(Guid userId, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            var query = _context.Posts
                .Where(x => x.AuthorUserId == userId)
                .AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword));
            }

            var totalRecords = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
        }
    }
}