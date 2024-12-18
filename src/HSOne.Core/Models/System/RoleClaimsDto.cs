using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class RoleClaimsDto
    {
        [Required]
        public required string Type { get; set; }
        [Required]
        public required string Value { get; set; }
        public string? DisplayName { get; set; }
        [Required]
        public bool IsSelected { get; set; }
    }
}