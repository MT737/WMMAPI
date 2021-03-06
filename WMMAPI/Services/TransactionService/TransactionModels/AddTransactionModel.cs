using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.TransactionServices.TransactionModels
{
    public class AddTransactionModel
    {                
        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid VendorId { get; set; }

        [Required, Range(0, 9999999999999999.99)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public Transaction ToDB(Guid userId)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TransactionDate = TransactionDate,
                AccountId = AccountId,
                CategoryId = CategoryId,
                VendorId = VendorId,
                Amount = Amount,
                Description = Description
            };
        }
    }
}