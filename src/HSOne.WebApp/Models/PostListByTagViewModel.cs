using HSOne.Core.Models.Content;
using HSOne.Core.Models;

namespace HSOne.WebApp.Models
{
    public class PostListByTagViewModel
    {
        public required PagedResult<PostInListDto> Posts { get; set; }
        public required TagDto Tag { get; set; }
    }
}
