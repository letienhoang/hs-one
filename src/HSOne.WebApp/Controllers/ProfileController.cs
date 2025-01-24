using HSOne.Core.Domain.Identity;
using HSOne.Core.SeedWorks;
using HSOne.WebApp.Extensions;
using HSOne.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public ProfileController(IUnitOfWork unitOfWork, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        private async Task<AppUser> GetUser()
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user!;
        }

        [Route("profile")]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser();
            return View(new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!
            });
        }

        [HttpGet]
        [Route("profile/edit")]
        public async Task<IActionResult> ChangeProfile()
        {
            var user = await GetUser();

            return View(new ChangeProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        [HttpPost]
        [Route("profile/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile([FromForm] ChangeProfileViewModel model)
        {
            var user = await GetUser();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Profile updated successfully";
            }
            else
            {
                ModelState.AddModelError("", "Failed to update profile");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("profile/change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("profile/change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetUser();
            
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!isPasswordCorrect)
            {
                ModelState.AddModelError("OldPassword", "Old password is incorrect");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password changed successfully";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
    }
}
