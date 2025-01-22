using HSOne.Core.SeedWorks;
using HSOne.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Controllers
{
    public class SeriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SeriesController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        [Route("series")]
        public async Task<IActionResult> Index([FromQuery] int page = 1)
        {
            var series = await _unitOfWork.Series.GetSeriesPagingAsync(string.Empty, page);
            return View(series);
        }

        [Route("series/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var series = await _unitOfWork.Series.GetSeriesBySlugAsync(slug);
            var posts = await _unitOfWork.Posts.GetAllPostBySeriesSlugPagingAsync(slug);
            return View(new SeriesDetailViewModel
            {
                Posts = posts,
                Series = series
            });
        }
    }
}
