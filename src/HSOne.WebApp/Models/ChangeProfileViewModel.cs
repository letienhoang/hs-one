using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HSOne.WebApp.Models
{
    public class ChangeProfileViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [DisplayName("First Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [DisplayName("Last Name")]
        public required string LastName { get; set; }
    }
}
