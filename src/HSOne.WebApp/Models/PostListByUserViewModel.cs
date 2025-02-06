using HSOne.Core.Models.Content;
using HSOne.Core.Models;
using HSOne.Core.Domain.Identity;

namespace HSOne.WebApp.Models
{
    public class PostListByUserViewModel
    {
        public PagedResult<PostInListDto>? Posts { get; set; }
        public string? UserName { get; set; }
    }
}
