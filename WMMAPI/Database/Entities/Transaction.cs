using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public class Transaction
    {
        //Properties   
        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public Guid TransactionTypeId { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid VendorId { get; set; }
        
        [Required, Range(-9999999999999999.99, 9999999999999999.99)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        
        //Navigation Properties
        public User User { get; set; }

        public TransactionType TransactionType { get; set; }
        
        public Account Account { get; set; }
        
        public Category Category { get; set; }
        
        public Vendor Vendor { get; set; }
    }
}
