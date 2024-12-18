using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class ChangeMyPasswordRequest
    {
        [Required]
        public required string CurrentPassword { get; set; }
        [Required]
        public required string NewPassword { get; set; }
    }
}
