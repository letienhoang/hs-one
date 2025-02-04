using AutoMapper;
using HSOne.Core.Domain.Content;

namespace HSOne.Core.Models.Content
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Slug { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Tag, TagDto>();
            }
        }
    }
}
