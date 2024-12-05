using HSOne.Api.Services;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public TokenController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticatedResult>> RefreshAsync(TokenRequest tokenRequest)
        {
            if (tokenRequest is null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            if (principal == null || principal.Identity == null || principal.Identity.Name == null)
            {
                return BadRequest("Invalid token");
            }

            var userName = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new AuthenticatedResult()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("revoke"), Authorize]
        public async Task<IActionResult> RevokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}