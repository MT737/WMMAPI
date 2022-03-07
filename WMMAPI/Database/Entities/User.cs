using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public class User
    {
        //Properties
        [Required]
        public Guid Id { get; set; }

        [Required, StringLength(200)]
        public string FirstName { get; set; }

        [Required, StringLength(200)]
        public string LastName { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        //Navigation Properties
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Vendor> Vendors { get; set; }
    }
}
