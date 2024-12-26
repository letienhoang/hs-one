using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Domain.Royalty
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }

        [MaxLength(256)]
        [Required]
        public required string FromUserName { get; set; }
        
        public Guid ToUserId { get; set; }

        [MaxLength(256)]
        [Required]
        public required string ToUserName { get; set; }

        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime DateCreated { get; set; }

        [MaxLength(256)]
        public string? Note { get; set; }
    }

    public enum TransactionType
    {
        RoyaltyPay
    }
}