using AutoMapper;
using HSOne.Core.Domain.Content;

namespace HSOne.Core.Models.Content
{
    public class PostInListDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Slug { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
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