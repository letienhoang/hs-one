using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class CreateUpdatePostCategoryRequest
    {
        [MaxLength(256)]
        [Required]
        public required string Name { set; get; }

        [Column(TypeName = "varchar(256)")]
        [Required]
        public required string Slug { set; get; }
        public Guid? ParentId { set; get; }
        [Required]
        public bool IsActive { set; get; }
        public string? SeoKeywords { set; get; }
        public string? SeoDescription { set; get; }
        [Required]
        public int SortOrder { set; get; }
        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<CreateUpdatePostCategoryRequest, PostCategory>();
            }
        }
    }
}
