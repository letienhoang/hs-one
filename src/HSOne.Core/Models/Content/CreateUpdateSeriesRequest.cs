using AutoMapper;
using HSOne.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSOne.Core.Models.Content
{
    public class CreateUpdateSeriesRequest
    {
        [Required]
        [MaxLength(256)]
        public required string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(256)")]
        public required string Slug { get; set; }
        [MaxLength(256)]
        public string? Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int SortOrder { get; set; }
        [MaxLength(160)]
        public string? SeoDescription { get; set; }
        [MaxLength(256)]
        public string? Thumbnail { set; get; }
        public string? Content { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<CreateUpdateSeriesRequest, Series>();
            }
        }
    }
}
