using AutoMapper;
using HSOne.Core.Domain.Identity;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class UserDto
    {
        [Required]
        public required Guid Id { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PhoneNumber { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public IList<string>? Roles { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public DateTime? Dob { get; set; }
        public string? Avatar { get; set; }
        public DateTime? VipStartDate { get; set; }
        public DateTime? VipExpireDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        [Required]
        public double Balance { get; set; }
        [Required]
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
