using HSOne.Core.SeedWorks;
using HSOne.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("posts")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("post/{slug}")]
        public IActionResult Detail([FromRoute] string slug)
        {
            return View();
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
        public IActionResult ListByTag([FromRoute] string tagSlug, [FromQuery] int? page = 1)
        {
            return View();
        }
    }
}