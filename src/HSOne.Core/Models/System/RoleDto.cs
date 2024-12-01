using AutoMapper;
using HSOne.Core.Domain.Identity;

namespace HSOne.Core.Models.System
{
    public class RoleDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
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