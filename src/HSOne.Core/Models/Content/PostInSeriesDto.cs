using AutoMapper;
using HSOne.Core.Domain.Content;

namespace HSOne.Core.Models.Content
{
    public class PostInSeriesDto
    {
        public Guid PostId { get; set; }
        public Guid SeriesId { get; set; }
        public int DisplayOrder { get; set; }
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<PostInSeries, PostInSeriesDto>();
            }
        }
    }
}
