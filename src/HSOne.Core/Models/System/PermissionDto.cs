using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class PermissionDto
    {
        [Required]
        public required string RoleId { get; set; }
        public required IList<RoleClaimsDto> RoleClaims { get; set; }
    }
}