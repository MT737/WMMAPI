using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.UserService.UserModels
{
    public class RegisterUserModel
    {
        //Properties
        [Required, StringLength(200)]
        public string FirstName { get; set; }

        [Required, StringLength(200)]
        public string LastName { get; set; }

        [Required]
        [SwaggerSchema(Format = "date")]
        public string DOB { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }

        [Required, StringLength(100)]
        public string Password { get; set; }

        public User ToDB()
        {
            User u = new()
            {
                Id = Guid.NewGuid(),
                FirstName = FirstName,
                LastName = LastName,
                DOB = DateTime.Parse(DOB),
                EmailAddress = EmailAddress
            };

            return u;
        }
    }
}
