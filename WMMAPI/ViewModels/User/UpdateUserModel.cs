using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.ViewModels.User
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


        public Database.Entities.User ToDB(Guid userId)
        {
            return new Database.Entities.User
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
