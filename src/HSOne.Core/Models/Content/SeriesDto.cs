using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class SeriesDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Slug { get; set; }
        public string? Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int SortOrder { get; set; }
        public string? SeoDescription { get; set; }
        public string? Thumbnail { set; get; }
        public string? Content { get; set; }
        [Required]
        public Guid AuthorUserId { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Series, SeriesDto>();
            }
        }
    }
}
