using AutoMapper;
using HSOne.Core.Domain.Identity;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models.System
{
    public class CreateUserRequest
    {
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
        public required string Password { get; set; }
        public DateTime? Dob { get; set; }
        public string? Avatar { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public double RoyaltyAmountPerPost { get; set; }
        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<CreateUserRequest, AppUser>();
            }
        }
    }
}
