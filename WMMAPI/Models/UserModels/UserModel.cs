using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.UserModels
{
    public class UserModel
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public string EmailAddress { get; set; }

        public UserModel(User user)
        {
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            DOB = user.DOB;
            EmailAddress = user.EmailAddress;
        }
    }
}
