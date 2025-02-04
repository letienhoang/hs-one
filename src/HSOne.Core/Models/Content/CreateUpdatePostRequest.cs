using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSOne.Core.Models.Content
{
    public class CreateUpdatePostRequest
    {
        [Required]
        [MaxLength(256)]
        public required string Title { get; set; }
        [Required]
        [Column(TypeName = "varchar(256)")]
        public required string Slug { get; set; }
        [Required]
        public Guid CategoryId { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
        [MaxLength(512)]
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        [MaxLength(512)]
        public string? Source { get; set; }
        [MaxLength(256)]
        public string[]? Tags { get; set; }
        [MaxLength(160)]
        public string? SeoDescription { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<CreateUpdatePostRequest, Post>()
                    .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.Tags != null ? string.Join(",", src.Tags) : string.Empty));
            }
        }
    }
}