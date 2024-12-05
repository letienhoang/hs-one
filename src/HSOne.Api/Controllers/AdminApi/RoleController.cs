using AutoMapper;
using HSOne.Api.Filters;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Models;
using HSOne.Core.Models.System;
using HSOne.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [ValidateModel]
        [Authorize(Permissions.Roles.Create)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateRoleRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var role = new AppRole()
            {
                Name = request.Name,
                DisplayName = request.DisplayName
            };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPut("update/{id}")]
        [ValidateModel]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateAsync(Guid id,[FromBody] CreateUpdateRoleRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            role.Name = request.Name;
            role.DisplayName = request.DisplayName;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpDelete]
        [Authorize(Permissions.Roles.Delete)]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("Invalid request");
            }

            foreach (var id in ids)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null)
                {
                    return NotFound();
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<RoleDto>> GetAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<RoleDto>(role);
            return Ok(response);
        }

        [HttpGet("paging")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PagedResult<RoleDto>>> GetPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword) || x.DisplayName.Contains(keyword));
            }

            var totalItems = query.Count();
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var roles = await _mapper.ProjectTo<RoleDto>(query).ToListAsync();
            var pagedResult = new PagedResult<RoleDto>
            {
                Results = roles,
                CurrentPage = pageIndex,
                RowCount = totalItems,
                PageSize = pageSize
            };

            return Ok(pagedResult);
        }
    }
}