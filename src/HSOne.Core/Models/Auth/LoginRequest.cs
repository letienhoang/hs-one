using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
