using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class SetPasswordRequest
    {
        [Required]
        public required string NewPassword { get; set; }
    }
}
