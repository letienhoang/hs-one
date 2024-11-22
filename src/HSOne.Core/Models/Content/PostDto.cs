using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;

namespace HSOne.Data.Repositories
{
    public class PostDto : PostInListDto
    {
        public Guid CategoryId { get; set; }
        public string? Content { get; set; }
        public Guid AuthorUserId { get; set; }
        public string? Source { get; set; }
        public string? Tags { get; set; }
        public string? SeoDescription { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsPaid { get; set; }
        public double RoyaltyAmount { get; set; }

        public class AutoMapperPostDtoProfile : Profile
        {
            public AutoMapperPostDtoProfile()
            {
                CreateMap<Post, PostDto>();
            }
        }
    }
}