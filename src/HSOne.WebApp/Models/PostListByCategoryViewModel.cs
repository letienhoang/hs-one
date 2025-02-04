using HSOne.Core.Models;
using HSOne.Core.Models.Content;

namespace HSOne.WebApp.Models
{
    public class PostListByCategoryViewModel
    {
        public required PagedResult<PostInListDto> Posts { get; set; }
        public required PostCategoryDto Category { get; set; }
    }
}
