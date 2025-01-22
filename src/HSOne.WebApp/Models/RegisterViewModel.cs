using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HSOne.WebApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [DisplayName("First Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [DisplayName("Last Name")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DisplayName("Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        public required string Password { get; set; }
    }
}
