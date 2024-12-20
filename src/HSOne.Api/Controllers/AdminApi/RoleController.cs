using AutoMapper;
using HSOne.Api.Extensions;
using HSOne.Api.Filters;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Models;
using HSOne.Core.Models.System;
using HSOne.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

        [HttpPost]
        [Route("create")]
        [ValidateModel]
        [Authorize(Permissions.Roles.Create)]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateUpdateRoleRequest request)
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

        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateRoleAsync(Guid id,[FromBody] CreateUpdateRoleRequest request)
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
        public async Task<IActionResult> DeleteRolesAsync([FromQuery] Guid[] ids)
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

                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<RoleDto>> GetRoleAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<RoleDto>(role);
            return Ok(response);
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PagedResult<RoleDto>>> GetRolesPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10)
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

        [HttpGet]
        [Route("all")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<List<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var response = _mapper.Map<List<RoleDto>>(roles);
            return Ok(response);
        }

        [HttpGet]
        [Route("{roleId}/permissions")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PermissionDto>> GetPermissionsAsync(string roleId)
        {
            var model = new PermissionDto()
            {
                RoleId = "",
                RoleClaims = new List<RoleClaimsDto>()
            };
            var allPermissions = new List<RoleClaimsDto>();
            var types = typeof(Permissions).GetTypeInfo().DeclaredNestedTypes;
            foreach(var type in types)
            {
                allPermissions.GetPermissions(type);
            }    

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            model.RoleId = role.Id.ToString();
            var claims = await _roleManager.GetClaimsAsync(role);
            var allClaimValues = allPermissions.Select(x => x.Value).ToList();
            var roleClaimValues = claims.Select(x => x.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permission in allPermissions)
            {
                if (authorizedClaims.Any(a=>a == permission.Value))
                {
                    permission.IsSelected = true;
                }
            }
            model.RoleClaims = allPermissions;

            return Ok(model);
        }

        [HttpPut]
        [Route("permissions")]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdatePermissionsAsync([FromBody] PermissionDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                return NotFound();
            }

            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            var selectedClaims = request.RoleClaims.Where(x => x.IsSelected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaimAsync(role, claim.Value);
            }

            return Ok();
        }
    }
}