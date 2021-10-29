using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.UserModels;

namespace WMMAPI.Controllers
{
    //TODO: Add additional exception handling...?

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appSettings;

        public UserController(ILogger<UserController> logger, IUserRepository userRepository, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(AuthenticatedUserModel), StatusCodes.Status200OK)]
        public IActionResult Authenticate([FromBody] AuthenticateUserModel model)
        {
            var user = _userRepository.Authenticate(model.EmailAddress, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email address and password combination is incorrect." });

            // Create an authentication token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new AuthenticatedUserModel(user, tokenString));
        }

        [AllowAnonymous]
        [HttpPut("register")]
        public IActionResult Register([FromBody] RegisterUserModel user)
        {
            try
            {
                // Create user
                User dbUser = user.ToDB();
                _userRepository.Create(dbUser, user.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // Return error message is exception thrown.
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        public IActionResult GetById(string id)
        {
            // Confirm user is the same requesting
            if (User.Identity.Name != id)
                return BadRequest(new { message = "Passed userId does not match id of authenticated user." });

            Guid userId = Guid.Parse(id);
            try
            {
                var result = _userRepository.GetById(userId);
                if (result != null)
                {
                    return Ok(new UserModel(result));
                }
                return BadRequest(new { message = "User not found" });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult ModifyUser(string id, [FromBody]UpdateUserModel model)
        {
            // Confirm user is the same requesting
            if (User.Identity.Name != id)
                return BadRequest(new { message = "Passed userId does not match id of authenticated user." });

            var dbUser = model.ToDB(Guid.Parse(id));
            try
            {
                //Update user
                _userRepository.Modify(dbUser, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // Return error message if there was an exception
                return BadRequest( new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(string id)
        {
            // Confirm user is the same requesting
            if (User.Identity.Name != id)
                return BadRequest(new { message = "Passed userId does not match id of authenticated user." });

            Guid userId = Guid.Parse(id);
            try
            {
                _userRepository.Delete(userId);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
