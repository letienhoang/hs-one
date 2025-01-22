using HSOne.Core.Models.Content;
using HSOne.Data.Repositories;

namespace HSOne.WebApp.Models
{
    public class PostDetailViewModel
    {
        public required PostDto Post { get; set; }
        public required PostCategoryDto Category { get; set; }
        public List<TagDto>? Tags { get; set; }
    }
}
