using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSOne.Core.Domain.Content
{
    [Table("Series")]
    [Index(nameof(Slug), IsUnique = true)]
    public class Series
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(256)]
        [Required]
        public required string Name { get; set; }

        [Column(TypeName = "varchar(256)")]
        public required string Slug { get; set; }

        [MaxLength(256)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        [MaxLength(160)]
        public string? SeoDescription { get; set; }

        [MaxLength(256)]
        public string? Thumbnail { set; get; }

        public string? Content { get; set; }
        public Guid AuthorUserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}