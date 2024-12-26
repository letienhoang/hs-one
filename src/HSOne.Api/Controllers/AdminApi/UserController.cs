using AutoMapper;
using HSOne.Api.Extensions;
using HSOne.Api.Filters;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Models;
using HSOne.Core.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static HSOne.Core.SeedWorks.Constants.Permissions;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Users.View)]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<AppUser, UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles;
            return Ok(userDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Users.Create)]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            if ((await _userManager.FindByNameAsync(request.UserName)) != null)
            {
                return BadRequest();
            }

            if ((await _userManager.FindByEmailAsync(request.Email)) != null)
            {
                return BadRequest();
            }
            var user = _mapper.Map<CreateUserRequest, AppUser>(request);
            user.DateCreated = DateTime.Now;
            user.LockoutEnabled = false;
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
        }

        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            _mapper.Map(request, user);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpDelete]
        [Authorize(Users.Delete)]
        public async Task<IActionResult> DeleteUsersAsync([FromQuery] string[] ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                await _userManager.DeleteAsync(user);
            }
            return Ok();
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(Users.View)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsersPagingAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                query = query.Where(x => x.FirstName.Contains(keyword)
                                         || x.UserName.Contains(keyword)
                                         || x.Email.Contains(keyword)
                                         || x.PhoneNumber.Contains(keyword));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            var totalRow = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
               .Skip((pageIndex - 1) * pageSize)
               .Take(pageSize);

            var pagedResponse = new PagedResult<UserDto>
            {
                Results = await _mapper.ProjectTo<UserDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return Ok(pagedResponse);
        }

        [HttpGet]
        [Authorize(Users.View)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsersAsync()
        {
            var query = _userManager.Users;
            query = query.OrderByDescending(x => x.DateCreated);
            return await _mapper.ProjectTo<UserDto>(query).ToListAsync();
        }

        [HttpPut]
        [Route("password-change-current-user")]
        [ValidateModel]
        public async Task<IActionResult> ChangePassWordAsync([FromBody] ChangeMyPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpPost]
        [Route("set-password/{id}")]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> SetPasswordAsync(Guid id, [FromBody] SetPasswordRequest model)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpPost]
        [Route("change-email/{id}")]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> ChangeEmailAsync(Guid id, [FromBody] ChangeEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.Email);
            var result = await _userManager.ChangeEmailAsync(user, request.Email, token);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpPut]
        [Route("assign-users/{id}")]
        [ValidateModel]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> AssignRolesAsync(string id, [FromBody] string[] roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removedResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var addedResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addedResult.Succeeded || !removedResult.Succeeded)
            {
                List<IdentityError> addedErrorList = addedResult.Errors.ToList();
                List<IdentityError> removedErrorList = removedResult.Errors.ToList();
                var errorList = new List<IdentityError>();
                errorList.AddRange(addedErrorList);
                errorList.AddRange(removedErrorList);

                return BadRequest(string.Join("<br/>", errorList.Select(x => x.Description)));
            }
            return Ok();
        }
    }
}