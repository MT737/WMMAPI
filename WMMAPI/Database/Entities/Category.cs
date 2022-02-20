﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public class Category
    {
        //Properties
        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }


        //Navigation Property
        public virtual User User { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
