using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.UserModels
{
    public class AuthenticatedUserModel
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public string EmailAddress { get; set; }

        public string Token { get; set; }
     
        public AuthenticatedUserModel(User user, string token)
        {
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            DOB = user.DOB;
            EmailAddress = user.EmailAddress;
            Token = token;
        }
    }

}
