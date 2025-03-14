﻿using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;
using HSOne.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HSOne.Core.SeedWorks.Constants;
using HSOne.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Helpers;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PostController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Permissions.Posts.View)]
        public async Task<ActionResult<PostDto>> GetPostAsync(Guid id)
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        [Authorize(Permissions.Posts.Create)]
        public async Task<ActionResult<PostDto>> CreatePostAsync([FromBody] CreateUpdatePostRequest request)
        {
            if (await _unitOfWork.Posts.IsSlugAlreadyExistedAsync(request.Slug))
            {
                return BadRequest("Slug already existed");
            }
            var post = _mapper.Map<CreateUpdatePostRequest, Post>(request);
            var category = await _unitOfWork.PostCategories.GetByIdAsync(request.CategoryId) ?? throw new Exception("Category does not exist");
            post.CategoryName = category.Name;
            post.CategorySlug = category.Slug;

            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("User does not exist");
            post.AuthorUserId = userId;
            post.AuthorUserName = user.UserName!;
            post.AuthorName = user.GetFullName();

            post.Status = PostStatus.Draft;

            post.Id = Guid.NewGuid();
            // Process Tag
            if (request.Tags != null && request.Tags.Length > 0)
            {
                foreach (var tag in request.Tags)
                {
                    var tagSlug = TextHelper.ToUnsignedString(tag);
                    var tagEntity = await _unitOfWork.Tags.GetTagBySlugAsync(tagSlug);
                    var tagId = tagEntity?.Id ?? Guid.NewGuid();
                    if (tagEntity == null)
                    {
                        _unitOfWork.Tags.Add(new Tag { Id = tagId, Name = tag, Slug = tagSlug });
                    }
                    await _unitOfWork.Tags.AddPostTagAsync(post.Id, tagId);
                }

                post.Tags = string.Join(",", request.Tags);
            }

            _unitOfWork.Posts.Add(post);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok(post) : BadRequest();
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Permissions.Posts.Edit)]
        public async Task<ActionResult<PostDto>> UpdatePostAsync(Guid id, [FromBody] CreateUpdatePostRequest request)
        {
            if (await _unitOfWork.Posts.IsSlugAlreadyExistedAsync(request.Slug, id))
            {
                return BadRequest("Slug already existed");
            }
            var post = await _unitOfWork.Posts.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.CategoryId != request.CategoryId)
            {
                var category = await _unitOfWork.PostCategories.GetByIdAsync(request.CategoryId) ?? throw new Exception("Category does not exist");
                post.CategoryName = category.Name;
                post.CategorySlug = category.Slug;
            } 
            // Process Tag
            if (request.Tags != null && request.Tags.Length > 0)
            {
                foreach (var tag in request.Tags)
                {
                    var tagSlug = TextHelper.ToUnsignedString(tag);
                    var tagEntity = await _unitOfWork.Tags.GetTagBySlugAsync(tagSlug);
                    var tagId = tagEntity?.Id ?? Guid.NewGuid();
                    if (tagEntity == null)
                    {
                        _unitOfWork.Tags.Add(new Tag { Id = tagId, Name = tag, Slug = tagSlug });
                    }
                    if (!await _unitOfWork.Tags.IsExistsPostTagAsync(id, tagId))
                    {
                        await _unitOfWork.Tags.AddPostTagAsync(id, tagId);
                    }
                }
            }

            _mapper.Map(request, post);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok(post) : BadRequest();
        }

        [HttpDelete]
        [Authorize(Permissions.Posts.Delete)]
        public async Task<ActionResult> DeletePostsAsync([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var post = await _unitOfWork.Posts.GetByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                if (await _unitOfWork.PostInSeries.HasSeriesContainingPostAsync(id))
                {
                    await _unitOfWork.PostInSeries.RemovePostInAllSeriesAsync(id);
                }

                if (await _unitOfWork.Tags.IsExistsTagOfPostAsync(id))
                {
                    await _unitOfWork.Tags.RemoveTagsOfPostAsync(id);
                }

                if (post.Thumbnail != null)
                {
                    var relativePath = post.Thumbnail.TrimStart('/').Replace("/", @"\");
                    var fullPath = Path.Combine(_hostingEnvironment.WebRootPath, relativePath);

                    if (!System.IO.File.Exists(fullPath))
                    {
                        return NotFound("File not found.");
                    }

                    System.IO.File.Delete(fullPath);
                }

                _unitOfWork.Posts.Remove(post);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(Permissions.Posts.View)]
        public async Task<ActionResult<PagedResult<PostInListDto>>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var userId = User.GetUserId();
            var posts = await _unitOfWork.Posts.GetPostsPagingAsync(keyword, userId, categoryId, pageIndex, pageSize);
            return Ok(posts);
        }

        [HttpGet]
        [Route("series-post")]
        [Authorize(Permissions.Posts.View)]
        public async Task<ActionResult<List<SeriesInListDto>>> GetAllSeriesForPostAsync(Guid postId)
        {
            var series = await _unitOfWork.Series.GetAllSeriesForPostAsync(postId);
            return Ok(series);
        }

        [HttpGet]
        [Route("post-in-series")]
        [Authorize(Permissions.Posts.View)]
        public async Task<ActionResult<PostInSeriesDto>> GetPostsInSeriesAsync(Guid postId, Guid seriesId)
        {
            var postInSeries = await _unitOfWork.PostInSeries.GetPostInSeriesAsync(postId, seriesId);
            return Ok(postInSeries);
        }

        [HttpPatch]
        [Route("approve/{id}")]
        [Authorize(Permissions.Posts.Approve)]
        public async Task<IActionResult> ApprovePostAsync(Guid id)
        {
            await _unitOfWork.Posts.ApproveAsync(id, User.GetUserId());
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpPatch]
        [Route("send-to-approve/{id}")]
        [Authorize(Permissions.Posts.Edit)]
        public async Task<IActionResult> SendForApprovalPostAsync(Guid id)
        {
            await _unitOfWork.Posts.SendForApprovalAsync(id, User.GetUserId());
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpPatch]
        [Route("reject/{id}")]
        [Authorize(Permissions.Posts.Approve)]
        public async Task<IActionResult> RejectPostAsync(Guid id, [FromBody] RejectPostRequest request)
        {
            await _unitOfWork.Posts.RejectAsync(id, User.GetUserId(), request.Reason);
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpGet]
        [Route("reject-reason/{id}")]
        [Authorize(Permissions.Posts.Approve)]
        public async Task<ActionResult<string>> GetRejectReasonAsync(Guid id)
        {
            var reason = await _unitOfWork.Posts.GetRejectReasonAsync(id);
            return Ok(reason);
        }

        [HttpGet]
        [Route("activity-logs/{id}")]
        [Authorize(Permissions.Posts.Approve)]
        public async Task<ActionResult<List<PostActivityLogDto>>> GetActivityLogsAsync(Guid id)
        {
            var logs = await _unitOfWork.Posts.GetActivityLogsAsync(id);
            return Ok(logs);

        }

        [HttpPatch]
        [Route("back-to-draft/{id}")]
        [Authorize(Permissions.Posts.Approve)]
        public async Task<IActionResult> BackToDraftAsync(Guid id)
        {
            await _unitOfWork.Posts.BackToDraftAsync(id, User.GetUserId());
            await _unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}