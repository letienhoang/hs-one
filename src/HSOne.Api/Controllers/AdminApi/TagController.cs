using AutoMapper;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;
using HSOne.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Permissions.Tags.View)]
        public async Task<ActionResult<List<TagDto>>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            return Ok(tags);
        }

        [HttpGet]
        [Route("tag-name")]
        [Authorize(Permissions.Tags.View)]
        public async Task<ActionResult<List<string>>> GetAllNameTagsAsync()
        {
            var tagName = await _unitOfWork.Tags.GetAllNameTagsAsync();
            return Ok(tagName);
        }
    }
}