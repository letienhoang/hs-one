using AutoMapper;
using HSOne.Core.Domain.Identity;

namespace HSOne.Core.Models.System
{
    public class UserDto
    {
        public required Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public IList<string>? Roles { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? Dob { get; set; }
        public string? Avatar { get; set; }
        public DateTime? VipStartDate { get; set; }
        public DateTime? VipExpireDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public double Balance { get; set; }
        public double RoyaltyAmountPerPost { get; set; }
        public string GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<AppUser, UserDto>();
            }
        }
    }
}
