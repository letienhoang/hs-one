using HSOne.Core.ConfigOptions;
using HSOne.Core.SeedWorks;
using HSOne.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HSOne.WebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<SystemConfig> _systemConfig;

        public PostController(IUnitOfWork unitOfWork, IOptions<SystemConfig> systemConfig)
        {
            _unitOfWork = unitOfWork;
            _systemConfig = systemConfig;
        }

        [Route("posts")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("post/{slug}")]
        public async Task<IActionResult> Detail([FromRoute] string slug)
        {
            var post = await _unitOfWork.Posts.GetBySlugAsync(slug);
            var category = await _unitOfWork.PostCategories.GetBySlugAsync(post.CategorySlug);
            var tags = await _unitOfWork.Tags.GetPostTagsAsync(post.Id);
            return View(new PostDetailViewModel
            {
                Post = post,
                Category = category,
                Tags = tags
            });
        }

        [Route("posts/{categorySlug}")]
        public async Task<IActionResult> ListByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1)
        {
            var posts = await _unitOfWork.Posts.GetPostsByCategoryPagingAsync(categorySlug, page);
            var category = await _unitOfWork.PostCategories.GetBySlugAsync(categorySlug);
            return View(new PostListByCategoryViewModel
            {
                Posts = posts,
                Category = category
            });
        }

        [Route("tag/{tagSlug}")]
        public async Task<IActionResult> ListByTag([FromRoute] string tagSlug, [FromQuery] int page = 1)
        {
            var posts = await _unitOfWork.Posts.GetPostsByTagPagingAsync(tagSlug, page);
            var tag = await _unitOfWork.Tags.GetTagBySlugAsync(tagSlug);
            return View(new PostListByTagViewModel
            {
                Posts = posts,
                Tag = tag!
            });
        }

        [HttpGet]
        [Route("post-thumbnail")]
        public async Task<IActionResult> PostThumbnail(Guid postId)
        {
            if (postId == Guid.Empty)
            {
                return NotFound("Post id is required.");
            }

            var post = await _unitOfWork.Posts.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }
            if (string.IsNullOrEmpty(post.Thumbnail))
            {
                return NotFound("Thumbnail not found.");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_systemConfig.Value.BackendApiUrl);
                var apiUrl = $"/api/admin/media?filePath={post.Thumbnail}";

                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound("Thumbnail not found.");
                }
                var image = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                return File(image, contentType);
            }
        }
    }
}