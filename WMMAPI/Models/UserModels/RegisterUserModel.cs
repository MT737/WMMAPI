using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.UserModels
{
    public class RegisterUserModel
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

        public User ToDB()
        {
            User u = new()
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
