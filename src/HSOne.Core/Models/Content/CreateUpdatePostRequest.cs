using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class CreateUpdatePostRequest
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Slug { get; set; }
        [Required]
        public Guid CategoryId { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public string? Source { get; set; }
        public string? Tags { get; set; }
        public string? SeoDescription { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<CreateUpdatePostRequest, Post>();
            }
        }
    }
}