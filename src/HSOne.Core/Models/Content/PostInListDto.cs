using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class PostInListDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Slug { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        [Required]
        public int ViewCount { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public PostStatus Status { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Post, PostInListDto>();
            }
        }
    }
}