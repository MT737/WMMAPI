using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WMMAPI.ViewModels.User
{
    public class NewUserViewModel
    {
        //Properties
        [Required, StringLength(200)]
        public string FirstName { get; set; }

        [Required, StringLength(200)]
        public string LastName { get; set; }

        [Required]
        public string DOB { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }

        //TODO: stringlength for a password?
        [Required, StringLength(100)]
        public string Password { get; set; }

        public Database.Models.User ToDB()
        {
            Database.Models.User u = new()
            {
                UserId = Guid.NewGuid(),
                FirstName = FirstName,
                LastName = LastName,
                DOB = DateTime.Parse(DOB),
                EmailAddress = EmailAddress
            };

            return u;
        }
    }
}
