using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.UserModels
{
    public class UpdateUserModel
    {
        [StringLength(200)]
        public string FirstName { get; set; }

        [StringLength(200)]
        public string LastName { get; set; }

        public string DOB { get; set; }

        [EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }
        
        [StringLength(100)]
        public string Password { get; set; }

        public User ToDB(Guid userId)
        {
            // Convert DOB
            DateTime dob;
            var conversion = DateTime.TryParse(DOB, out dob);

            return new User
            {
                Id = userId,
                FirstName = FirstName ?? " ",
                LastName = LastName ?? " ",
                DOB = conversion ? dob : DateTime.MinValue,
                EmailAddress = EmailAddress ?? " "
            };
        }
    }
}
