using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Database.Models
{
    public class User
    {
        //Properties
        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(200)]
        public string FirstName { get; set; }

        [Required, StringLength(200)]
        public string LastName { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }
    }
}
