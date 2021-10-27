using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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


        public Database.Models.User ToDB(Guid userId)
        {
            return new Database.Models.User
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
