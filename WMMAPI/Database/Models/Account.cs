using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Models
{
    public class Account
    {
        //Properties
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsAsset { get; set; }

        [Required]
        public bool IsActive { get; set; }


        //Navigation Property
        public User User { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
