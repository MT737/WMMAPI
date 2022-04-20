using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Services.UserService.UserModels
{
    public class AuthenticateUserModel
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
