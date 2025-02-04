using HSOne.Core.Models.Content;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HSOne.WebApp.Models
{
    public class CreatePostViewModel
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Content { get; set; }
        public string? ThumbnailImage { get; set; }
        public Guid CategoryId { get; set; }
        public SelectList? Categories { get; set; }
        public string? SeoDescription { get; set; }
        public string? Source { get; set; }
    }
}
