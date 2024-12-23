using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Content
{
    public class RejectPostRequest
    {
        [Required]
        public required string Reason { set; get; }
    }
}
