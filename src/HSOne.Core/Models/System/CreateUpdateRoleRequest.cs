using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class CreateUpdateRoleRequest
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string DisplayName { get; set; }
    }
}