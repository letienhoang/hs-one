using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Domain.Royalty
{
    public class TransactionDto
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        [Required]
        public required string FromUserName { get; set; }
        
        public Guid ToUserId { get; set; }
        [Required]
        public required string ToUserName { get; set; }

        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Note { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Transaction, TransactionDto>();
                CreateMap<TransactionDto, Transaction>();
            }
        }
    }
}