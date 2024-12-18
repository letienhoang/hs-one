using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class ChangeEmailRequest
    {
        [Required]
        public required string Email { get; set; }
    }
}