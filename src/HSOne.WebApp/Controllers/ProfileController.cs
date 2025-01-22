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

        [Route("profile")]
        public async Task<IActionResult> Index()
        {
            var userId = User.GetUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return View(new ProfileViewModel
            {
                FirstName = user!.FirstName,
                LastName = user.LastName,
                Email = user.Email!
            });
        }

        [HttpGet]
        [Route("profile/edit")]
        public async Task<IActionResult> ChangeProfile()
        {
            TempData["Success"] = null;
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return View(new ChangeProfileViewModel
            {
                FirstName = user!.FirstName,
                LastName = user.LastName
            });
        }

        [HttpPost]
        [Route("profile/edit")]
        public async Task<IActionResult> ChangeProfile([FromForm] ChangeProfileViewModel model)
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user!.FirstName = model.FirstName;
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
    }
}
