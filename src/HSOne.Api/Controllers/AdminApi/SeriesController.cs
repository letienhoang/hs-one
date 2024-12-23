using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;
using HSOne.Core.Models;
using HSOne.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HSOne.Core.SeedWorks.Constants;
using HSOne.Api.Extensions;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SeriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Permissions.Series.Create)]
        public async Task<IActionResult> CreateSeriesAsync([FromBody] CreateUpdateSeriesRequest request)
        {
            var series = _mapper.Map<CreateUpdateSeriesRequest, Series>(request);
            series.AuthorUserId = User.GetUserId();
            _unitOfWork.Series.Add(series);

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpPut]
        [Authorize(Permissions.Series.Edit)]
        public async Task<IActionResult> UpdateSeriesAsync(Guid id, [FromBody] CreateUpdateSeriesRequest request)
        {
            var series = await _unitOfWork.Series.GetByIdAsync(id);
            if (series == null)
            {
                return NotFound();
            }
            _mapper.Map(request, series);

            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpDelete]
        [Authorize(Permissions.Series.Delete)]
        public async Task<IActionResult> DeleteSeriesAsync([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var postCategory = await _unitOfWork.Series.GetByIdAsync(id);
                if (postCategory == null)
                {
                    return NotFound();
                }
                _unitOfWork.Series.Remove(postCategory);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Permissions.Series.View)]
        public async Task<ActionResult<SeriesDto>> GetSeriesAsync(Guid id)
        {
            var series = await _unitOfWork.Series.GetByIdAsync(id);
            if (series == null)
            {
                return NotFound();
            }
            var seriesDto = _mapper.Map<SeriesDto>(series);
            return Ok(seriesDto);
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(Permissions.Series.View)]
        public async Task<ActionResult<PagedResult<SeriesInListDto>>> GetSeriesPagingAsync(string? keyword, int pageIndex, int pageSize = 10)
        {
            var result = await _unitOfWork.Series.GetSeriesPagingAsync(keyword, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Permissions.Series.View)]
        public async Task<ActionResult<List<SeriesDto>>> GetAllSeriesAsync()
        {
            var query = await _unitOfWork.Series.GetAllAsync();
            var model = _mapper.Map<List<SeriesDto>>(query);
            return Ok(model);
        }

        [HttpPost]
        [Route("post-series")]
        [Authorize(Permissions.Series.Edit)]
        public async Task<IActionResult> AddPostSeriesAsync([FromBody] AddPostSeriesRequest request)
        {
            var isExisted = await _unitOfWork.PostInSeries.IsPostInSeriesAsync(request.PostId, request.SeriesId);
            if (isExisted)
            {
                return BadRequest("Post is already in series");
            }
            await _unitOfWork.PostInSeries.AddPostToSeriesAsync(request.SeriesId, request.PostId, request.SortOrder);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpDelete]
        [Route("post-series")]
        [Authorize(Permissions.Series.Edit)]
        public async Task<IActionResult> DeletePostSeriesAsync([FromBody] AddPostSeriesRequest request)
        {
            var isExisted = await _unitOfWork.PostInSeries.IsPostInSeriesAsync(request.PostId, request.SeriesId);
            if (!isExisted)
            {
                return BadRequest("Post is not in series");
            }
            await _unitOfWork.PostInSeries.RemovePostToSeriesAsync(request.SeriesId, request.PostId);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("post-series/{seriesId}")]
        [Authorize(Permissions.Series.View)]
        public async Task<ActionResult<List<PostInListDto>>> GetAllPostInSeriesAsync(Guid seriesId)
        {
            var posts = await _unitOfWork.Posts.GetAllPostsInSeriesAsync(seriesId);
            return Ok(posts);
        }
    }
}
