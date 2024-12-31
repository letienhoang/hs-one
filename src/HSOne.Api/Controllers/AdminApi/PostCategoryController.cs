using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;
using HSOne.Core.Models;
using HSOne.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HSOne.Core.SeedWorks.Constants.Permissions;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/post-categoty")]
    [ApiController]
    public class PostCategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PostCategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(PostCategories.Create)]
        public async Task<IActionResult> CreatePostCategoryAsync([FromBody] CreateUpdatePostCategoryRequest request)
        {
            var postCategory = _mapper.Map<CreateUpdatePostCategoryRequest, PostCategory>(request);

            _unitOfWork.PostCategories.Add(postCategory);

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpPut]
        [Authorize(PostCategories.Edit)]
        public async Task<IActionResult> UpdatePostCategoryAsync(Guid id, [FromBody] CreateUpdatePostCategoryRequest request)
        {
            var postCategory = await _unitOfWork.PostCategories.GetByIdAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            _mapper.Map(request, postCategory);

            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpDelete]
        [Authorize(PostCategories.Delete)]
        public async Task<IActionResult> DeletePostCategoriesAsync([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var postCategory = await _unitOfWork.PostCategories.GetByIdAsync(id);
                if (postCategory == null)
                {
                    return NotFound();
                }
                if (await _unitOfWork.Posts.HasPostsInCategoryAsync(id))
                {
                    return BadRequest("Category has posts");
                }
                _unitOfWork.PostCategories.Remove(postCategory);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(PostCategories.View)]
        public async Task<ActionResult<PostCategoryDto>> GetPostCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.PostCategories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDto = _mapper.Map<PostCategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(PostCategories.View)]
        public async Task<ActionResult<PagedResult<PostCategoryDto>>> GetPostCategoriesPagingAsync(string? keyword, int pageIndex, int pageSize = 10)
        {
            var result = await _unitOfWork.PostCategories.GetPostCategoriesPagingAsync(keyword, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(PostCategories.View)]
        public async Task<ActionResult<List<PostCategoryDto>>> GetAllPostCategoriesAsync()
        {
            var query = await _unitOfWork.PostCategories.GetAllAsync();
            var model = _mapper.Map<List<PostCategoryDto>>(query);
            return Ok(model);
        }
    }
}
