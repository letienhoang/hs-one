using HSOne.Core.Domain.Content;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class SeriesInListDto
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
        [Required]
        public Guid AuthorUserId { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Series, SeriesInListDto>();
            }
        }
    }
}
