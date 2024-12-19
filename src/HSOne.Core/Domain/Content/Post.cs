using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSOne.Core.Domain.Content
{
    [Table("Posts")]
    [Index(nameof(Slug), IsUnique = true)]
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(256)]
        public required string Title { get; set; }

        [Column(TypeName = "varchar(256)")]
        public required string Slug { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }

        public Guid CategoryId { get; set; }

        [MaxLength(512)]
        public string? Thumbnail { get; set; }

        public string? Content { get; set; }

        [MaxLength(512)]
        public Guid AuthorUserId { get; set; }

        [MaxLength(512)]
        public string? Source { get; set; }

        [MaxLength(256)]
        public string? Tags { get; set; }

        [MaxLength(160)]
        public string? SeoDescription { get; set; }

        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsPaid { get; set; }
        public double RoyaltyAmount { get; set; }
        public PostStatus Status { get; set; }

        [Column(TypeName = "varchar(256)")]
        public required string CategorySlug { get; set; }
        [MaxLength(256)]
        public required string CategoryName { get; set; }
        [MaxLength(256)]
        public required string AuthorUserName { get; set; }
        [MaxLength(256)]
        public required string AuthorName { get; set; }
    }

    public enum PostStatus
    {
        Draft = 1,
        Canceled = 2,
        WaitingForApproval = 3,
        Rejected = 4,
        WaitingForPublish = 5,
        Published = 6
    }
}