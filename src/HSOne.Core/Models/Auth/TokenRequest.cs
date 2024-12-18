using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.Auth
{
    public class TokenRequest
    {
        [Required]
        public required string AccessToken { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
    }
}