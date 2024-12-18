using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Auth
{
    public class AuthenticatedResult
    {
        [Required]
        public required string Token { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
    }
}