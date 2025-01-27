using HSOne.Core.ConfigOptions;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Events.LoginSuccessed;
using HSOne.Core.Events.RegisterSuccessed;
using HSOne.Core.SeedWorks.Constants;
using HSOne.WebApp.Extensions;
using HSOne.WebApp.Models;
using HSOne.WebApp.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HSOne.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMediator _mediator;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<SystemConfig> _systemConfig;
        public AuthController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IMediator mediator, 
            IEmailSender emailSender,
            IOptions<SystemConfig> systemConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _emailSender = emailSender;
            _systemConfig = systemConfig;
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

        [HttpGet]
        [Route("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("forgot-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordViewModel model)
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

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), token, Request.Scheme);

            //EmailData emailData = new EmailData
            //{
            //    ToEmail = model.Email,
            //    Subject = $"{_systemConfig.AppName} - Retrieve password",
            //    Content = $"Hi {user.FirstName}. </br>" +
            //    $"You have just requested to retrieve your password at {_systemConfig.AppName}. </br>" +
            //    $"Please click <a href='{callbackUrl}'>here</a> to reset your password. </br>" +
            //    $"Thank you.",
            //};

            //await _emailSender.SendEmail(emailData);

            TempData[SystemConsts.FormSuccessMessage] = "Please check your email to reset password";
            return Redirect(UrlConsts.Login);
        }

        [HttpGet]
        [Route("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            if(token == null)
            {
                ModelState.AddModelError("", "Invalid token");
                return View();
            }
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = "",
                Password = "",
                ConfirmPassword = ""
            };
            return View(model);
        }

        [HttpPost]
        [Route("reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
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

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData[SystemConsts.FormSuccessMessage] = "Password has been reset successfully";
                return Redirect(UrlConsts.Login);
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
    }
}
