namespace HSOne.Core.Models.System
{
    public class PermissionDto
    {
        public required string RoleId { get; set; }
        public required IList<RoleClaimsDto> RoleClaims { get; set; }
    }
}