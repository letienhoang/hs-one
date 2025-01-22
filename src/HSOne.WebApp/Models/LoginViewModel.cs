using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HSOne.WebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DisplayName("Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
