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

        public DateTime? DOB { get; set; }

        [EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }
        
        //TODO: stringlength for a password?
        [StringLength(100)]
        public string Password { get; set; }


        public User ToDB(Guid userId)
        {
            return new User
            {
                UserId = userId,
                FirstName = FirstName ?? " ",
                LastName = LastName ?? " ",
                DOB = DOB ?? DateTime.MinValue,
                EmailAddress = EmailAddress ?? " "
            };
        }
    }
}
