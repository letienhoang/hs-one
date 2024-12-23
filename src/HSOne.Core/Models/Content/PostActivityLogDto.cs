using AutoMapper;
using HSOne.Core.Domain.Content;

namespace HSOne.Core.Models.Content
{
    public class PostActivityLogDto
    {
        public PostStatus FromStatus { set; get; }
        public PostStatus ToStatus { set; get; }
        public DateTime DateCreated { get; set; }
        public string? Note { set; get; }
        public Guid UserId { get; set; }
        public required string UserName { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<PostActivityLog, PostActivityLogDto>();
            }
        }
    }
}
