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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public UserController(ILogger<UserController> logger, IUserService userService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(AuthenticatedUserModel), StatusCodes.Status200OK)]
        public IActionResult AuthenticateUser([FromBody] AuthenticateUserModel model)
        {
            var user = _userService.Authenticate(model.EmailAddress, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email address and password combination is incorrect." });

            // Create an authentication token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
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
        public IActionResult RegisterUser([FromBody] RegisterUserModel user)
        {
            try
            {
                User dbUser = user.ToDB();
                _userService.Create(dbUser, user.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            try
            {
                var result = _userService.GetById(Guid.Parse(User.Identity.Name));
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

        [HttpPut]
        public IActionResult ModifyUser([FromBody] UpdateUserModel model)
        {            
            try
            {
                var dbUser = model.ToDB(Guid.Parse(User.Identity.Name));    
                _userService.Modify(dbUser, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest( new { message = ex.Message });
            }
        }

        [HttpDelete]
        public IActionResult DeleteUser()
        {            
            try
            {
                _userService.RemoveUser(Guid.Parse(User.Identity.Name));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
