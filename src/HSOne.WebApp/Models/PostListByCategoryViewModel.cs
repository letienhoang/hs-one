using HSOne.Core.Models;
using HSOne.Core.Models.Content;

namespace HSOne.WebApp.Models
{
    public class PostListByCategoryViewModel
    {
        public PagedResult<PostInListDto>? Posts { get; set; }
        public PostCategoryDto? Category { get; set; }
    }
}
