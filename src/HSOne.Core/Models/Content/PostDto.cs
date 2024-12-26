using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Data.Repositories
{
    public class PostDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Slug { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public string? Content { get; set; }
        [Required]
        public Guid AuthorUserId { get; set; }
        public string? Source { get; set; }
        public string? Tags { get; set; }
        public string? SeoDescription { get; set; }
        public DateTime? DateModified { get; set; }
        
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        [Required]
        public int ViewCount { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public PostStatus Status { get; set; }

        [Required]
        public required string CategorySlug { get; set; }
        [Required]
        public required string CategoryName { get; set; }
        [Required]
        public required string AuthorUserName { get; set; }
        [Required]
        public required string AuthorName { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        public double RoyaltyAmount { get; set; }
        public DateTime? PaidDate { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Post, PostDto>();
            }
        }
    }
}