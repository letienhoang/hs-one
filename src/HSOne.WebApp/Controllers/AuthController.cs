using HSOne.Core.Domain.Identity;
using HSOne.Core.Events.LoginSuccessed;
using HSOne.Core.Events.RegisterSuccessed;
using HSOne.Core.SeedWorks.Constants;
using HSOne.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMediator _mediator;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            var result = await _userManager.CreateAsync(new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            }, model.Password);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View();
                }
                await _signInManager.SignInAsync(user, isPersistent: true);
                await _mediator.Publish(new RegisterSuccessedEvent(model.Email));
                return Redirect(UrlConsts.Profile);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email not found");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                await _mediator.Publish(new LoginSuccessedEvent(model.Email));
                return Redirect(UrlConsts.Profile);
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password");
            }

            return View();
        }
    }
}
