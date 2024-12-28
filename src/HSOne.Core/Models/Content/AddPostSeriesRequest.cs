using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class AddPostSeriesRequest
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public Guid SeriesId { get; set; }
        public int SortOrder { get; set; }
    }
}
