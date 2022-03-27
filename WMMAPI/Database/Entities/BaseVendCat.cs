using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Entities
{
    public abstract class BaseVendCat
    {
        //Properties
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }

    }
}