using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public class Vendor
    {
        //Properties
        [Required]
        public Guid VendorId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }


        //Navigation Property
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual User User { get; set; }
    }
}
