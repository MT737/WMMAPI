using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public class Transaction
    {
        //Properties   
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid VendorId { get; set; }

        [Required]
        public bool IsDebit { get; set; }

        [Required, Range(0, 9999999999999999.99), Precision(16, 2)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        
        //Navigation Properties
        public virtual User User { get; set; }
        
        public virtual Account Account { get; set; }
        
        public virtual Category Category { get; set; }
        
        public virtual Vendor Vendor { get; set; }
    }
}
