using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.UserService.UserModels
{
    public class AuthenticatedUserModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public string EmailAddress { get; set; }

        public string Token { get; set; }
     
        public AuthenticatedUserModel(User user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            DOB = user.DOB;
            EmailAddress = user.EmailAddress;
            Token = token;
        }
    }

}
