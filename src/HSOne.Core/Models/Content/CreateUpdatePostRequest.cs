using AutoMapper;
using HSOne.Core.Domain.Content;

namespace HSOne.Core.Models.Content
{
    public class CreateUpdatePostRequest
    {
        public required string Title { get; set; }
        public required string Slug { get; set; }
        public Guid CategoryId { get; set; }
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