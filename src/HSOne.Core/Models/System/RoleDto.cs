using AutoMapper;
using HSOne.Core.Domain.Identity;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class RoleDto
    {
        [Required]
        public required string Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string DisplayName { get; set; }
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<AppRole, RoleDto>();
            }
        }
    }
}