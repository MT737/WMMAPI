using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public class TransactionType
    {
        //Properties
        [Required]
        public Guid TransactionTypeId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }


        //Navigation Property
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
