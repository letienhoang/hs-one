using HSOne.Core.Domain.Content;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Helpers;
using HSOne.Core.SeedWorks;
using HSOne.Core.SeedWorks.Constants;
using HSOne.Data.Repositories;
using HSOne.WebApp.Extensions;
using HSOne.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using HSOne.Core.ConfigOptions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HSOne.WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<SystemConfig> _systemConfig;
        public ProfileController(IUnitOfWork unitOfWork, 
            SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager,
            IOptions<SystemConfig> systemConfig)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _userManager = userManager;
            _systemConfig = systemConfig;
        }

        private async Task<AppUser> GetUser()
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user!;
        }

        private async Task UploadThumbnail(IFormFile thumbnailFile, Post post)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_systemConfig.Value.BackendApiUrl);

                byte[] data;
                using (var br = new BinaryReader(thumbnailFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)thumbnailFile.OpenReadStream().Length);
                }

                var bytes = new ByteArrayContent(data);

                var multiContent = new MultipartFormDataContent
                {
                    { bytes, "file", thumbnailFile.FileName }
                };

                var uploadResult = await client.PostAsync($"api/admin/media?type=posts&newFileName={post.Slug}", multiContent);
                if (uploadResult.StatusCode != HttpStatusCode.OK)
                {
                    ModelState.AddModelError("", await uploadResult.Content.ReadAsStringAsync());
                }
                else
                {
                    var path = await uploadResult.Content.ReadAsStringAsync();
                    var pathObj = JsonSerializer.Deserialize<UploadResponse>(path);
                    post.Thumbnail = pathObj?.Path;
                }

            }
        }

        private async Task<CreatePostViewModel> SetCreatePostViewModelAsync()
        {
            var categories = await _unitOfWork.PostCategories.GetAllAsync();
            return new CreatePostViewModel()
            {
                Title = "",
                Description = "",
                Content = "",
                Categories = new SelectList(categories, "Id", "Name")
            };
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
                TempData[SystemConsts.FormSuccessMessage] = "Profile updated successfully";
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
                TempData[SystemConsts.FormSuccessMessage] = "Password changed successfully";
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

        [HttpGet]
        [Route("profile/posts/list")]
        public async Task<IActionResult> PostListByUser(string keyword, int page = 1)
        {
            var posts = await _unitOfWork.Posts.GetPostsByUserPagingAsync(User.GetUserId(), keyword, page, 12);
            var userName = User.GetUserName();
            return View(new PostListByUserViewModel { 
                Posts = posts,
                UserName = userName
            });
        }

        [HttpGet]
        [Route("profile/posts/create")]
        public async Task<IActionResult> CreatePost()
        {
            return View(await SetCreatePostViewModelAsync());
        }

        [HttpPost]
        [Route("profile/posts/create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostViewModel model, IFormFile thumbnailFile)
        {
            if (!ModelState.IsValid)
            {
                return View(await SetCreatePostViewModelAsync());
            }
            var user = await GetUser();
            var category = await _unitOfWork.PostCategories.GetByIdAsync(model.CategoryId);
            var post = new Post
            {
                Title = model.Title,
                CategoryId = category!.Id,
                CategoryName = category.Name,
                CategorySlug = category.Slug,
                Slug = TextHelper.ToUnsignedString(model.Title),
                Content = model.Content,
                SeoDescription = model.SeoDescription,
                Source = model.Source,
                Status = PostStatus.Draft,
                AuthorUserId = user.Id,
                AuthorName = user.GetFullName(),
                AuthorUserName = user.UserName!,
                Description = model.Description,
            };
            _unitOfWork.Posts.Add(post);
            if (thumbnailFile != null)
            {
                await UploadThumbnail(thumbnailFile, post);
            }
            var result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                TempData[SystemConsts.FormSuccessMessage] = "Post created successfully";
            }
            else
            {
                ModelState.AddModelError("", "Failed to create post");
            }

            return RedirectToAction("Index");
        }
    }
}
