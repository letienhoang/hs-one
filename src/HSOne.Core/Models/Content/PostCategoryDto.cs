using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class PostCategoryDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public required string Name { set; get; }
        [Required]
        public required string Slug { set; get; }
        public Guid? ParentId { set; get; }
        [Required]
        public bool IsActive { set; get; }
        [Required]
        public DateTime DateCreated { set; get; }
        public DateTime? DateModified { set; get; }
        public string? SeoKeywords { set; get; }
        public string? SeoDescription { set; get; }
        [Required]
        public int SortOrder { set; get; }
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<PostCategory, PostCategoryDto>();
            }
        }
    }
}
