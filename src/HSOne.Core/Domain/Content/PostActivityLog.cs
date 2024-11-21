using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSOne.Core.Domain.Content
{
    [Table("PostActivityLogs")]
    public class PostActivityLog
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public PostStatus FromStatus { set; get; }

        public PostStatus ToStatus { set; get; }

        public DateTime DateCreated { get; set; }

        [MaxLength(512)]
        public string? Note { set; get; }

        public Guid UserId { get; set; }
    }
}