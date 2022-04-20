using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.TransactionServices.TransactionModels
{
    public class TransactionModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public Guid VendorId { get; set; }

        [Required]
        public string Vendor { get; set; }

        [Required]
        public bool IsDebit { get; set; }

        [Required, Range(0, 9999999999999999.99)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public TransactionModel(Transaction transaction)
        {
            Id = transaction.Id;
            TransactionDate = transaction.TransactionDate;
            AccountId = transaction.AccountId;
            Account = transaction.Account.Name;
            CategoryId = transaction.CategoryId;
            Category = transaction.Category.Name;
            VendorId = transaction.VendorId;
            Vendor = transaction.Vendor.Name;
            IsDebit = transaction.IsDebit;
            Amount = transaction.Amount;
            Description = transaction.Description;
        }

        public Transaction ToDB(Guid userId)
        {
            return new Transaction
            {
                UserId = userId,
                IsDebit = IsDebit,
                AccountId = AccountId,
                CategoryId = CategoryId,
                VendorId = VendorId,
                Amount = Amount,
                Description = Description
            };
        }
    }
}
