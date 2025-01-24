using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using HSOne.WebApp.Extensions;

namespace HSOne.WebApp.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old password is required")]
        [DisplayName("Old Password")]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DisplayName("New Password")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Comfirm new password is required")]
        [DisplayName("Comfirm New Password")]
        [PasswordMatch("NewPassword", ErrorMessage = "Password and Confirm Password must match")]
        public required string ComfirmNewPassword { get; set; }
    }
}
